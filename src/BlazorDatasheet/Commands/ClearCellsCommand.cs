﻿using BlazorDatasheet.Data;
using BlazorDatasheet.Interfaces;

namespace BlazorDatasheet.Commands;

/// <summary>
/// Clears cell values in the given ranges
/// </summary>
public class ClearCellsCommand : IUndoableCommand
{
    private readonly BRange _range;
    private readonly List<CellChange> _clearCommandOccurrences;

    public ClearCellsCommand(BRange range)
    {
        _clearCommandOccurrences = new List<CellChange>();
        _range = range.Clone();
    }

    public bool Execute(Sheet sheet)
    {
        foreach (var cell in _range.GetNonEmptyCells())
        {
            var oldValue = cell.GetValue();

            // When this is redone it'll update the new value to the old value.
            if (oldValue != null && !string.IsNullOrEmpty(oldValue.ToString()))
            {
                _clearCommandOccurrences.Add(
                    new CellChange(cell.Row, cell.Col, oldValue));
            }
        }

        // There were no empty cells in range so we can't clear anything
        if (!_clearCommandOccurrences.Any())
            return false;

        sheet.ClearCellsImpl(_range);
        return true;
    }

    public bool Undo(Sheet sheet)
    {
        sheet.Selection.Set(_range);
        sheet.SetCellValuesImpl(_clearCommandOccurrences);
        return true;
    }
}