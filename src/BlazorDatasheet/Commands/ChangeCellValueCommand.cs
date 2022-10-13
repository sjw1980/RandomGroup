﻿using BlazorDatasheet.Data;

namespace BlazorDatasheet.Commands;

public class ChangeCellValueCommand : IUndoableCommand
{
    private readonly int _row;
    private readonly int _col;
    private readonly object? _newValue;
    private object? _oldValue;
    
    public ChangeCellValueCommand(int row, int col, object value)
    {
        _row = row;
        _col = col;
        _newValue = value;
    }
    public bool Execute(Sheet sheet)
    {
        var cell = sheet.GetCell(_row, _col);
        if (cell == null)
            _oldValue = null;
        else
            _oldValue = cell.GetValue();
        
        return sheet.TrySetCellValue(_row, _col, _newValue);
    }

    public bool Undo(Sheet sheet)
    {
        return sheet.TrySetCellValue(_row, _col, _oldValue);
    }
}