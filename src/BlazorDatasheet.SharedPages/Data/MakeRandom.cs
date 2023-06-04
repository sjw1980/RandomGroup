using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using static BlazorDatasheet.SharedPages.Data.Log;

namespace BlazorDatasheet.SharedPages.Data
{
    internal class MakeRandom
    {
        public static int MaximumInGroup { get; set; } = 0; // 조당 최대 인원
        public static int ManualIndex { get; set; } = 0; // 미참석 컬럼 Index
        public static int MorePeopleIndex { get; set; } = 0; // 짝궁 컬럼 Index
        public static int DateGroupRankNumber { get; set; } = 0; // 날짜 기준 몇개 그룹으로 할 것인가?

        public static int DateGroupIndex { get; set; } = Constants.NO_EXIST_COLUMN; // 미 확인 컬럼 Index
        public static int IndexID_Column { get; set; } = 0;

        public const int ColumnNameRow = 1;

        // struct name and value
        public static (string userIndex, string value) MakeStruct(string userIndex, string value)
        {
            return (userIndex, value);
        }

        public static string[,] MemberRawInfo = SampleRawInfo.Data;

        public static dynamic objResult = new ExpandoObject();
        public static string errorDetail = "";

        public static int TotalAttribute;
        public static int MemberRawLength;
        public static int LackGroups = 0;

        public static Dictionary<string, string> MemberIdToLineNumber = new Dictionary<string, string>();

        public static List<string[]> MemberInfo = new List<string[]>();
        public static int[] ErrorCount = new int[TotalAttribute];

        public static int MemberLength;
        public static List<int> MemberList = new List<int>();
        public static Dictionary<string, Limit> Limits = new Dictionary<string, Limit>();

        public static Dictionary<int, double> ImportantAttribute = new Dictionary<int, double>();

        public static List<Dictionary<string, int>> MemberData = new List<Dictionary<string, int>>();
        public static Dictionary<string, int> ElementsAmount = new Dictionary<string, int>();

        public static List<Group> AllGroup = new List<Group>();
        public static int TotalGroupLength;
        public static int AllMemberWithFamily;
        public static int TotalFamilyMembers;

        public static int PassGroup = 0;
        public static int FailGroup = 0;
        public static int UnderGroup = 0;
        public static int OverGroup = 0;
        public static int FitGroup = 0;

        public static void Init()
        {
            PassGroup = 0;
            FailGroup = 0;
            UnderGroup = 0;
            OverGroup = 0;
            FitGroup = 0;

            objResult = new ExpandoObject();
            TotalAttribute = MemberRawInfo.Length;
            MemberRawLength = MemberRawInfo.Length - ColumnNameRow - 1;
            LackGroups = 0;

            MemberIdToLineNumber.Clear();

            MemberInfo.Clear();
            Array.Fill(ErrorCount, 0);

            MemberList.Clear();
            Limits.Clear();

            ImportantAttribute.Clear();

            MemberData.Clear();
            ElementsAmount.Clear();

            if (Log.Config.PreSummary)
            {
                Console.WriteLine("Total Attribute: " + TotalAttribute);
                Console.WriteLine("Total Raw Member: " + MemberRawLength);
            }

            for (int MemberIndex = 2; MemberIndex < MemberRawInfo.Length; MemberIndex++)
            {
                MemberIdToLineNumber[MemberRawInfo[MemberIndex, IndexID_Column]] = MemberIndex.ToString();

                bool isMemberPush = false;
                if (ManualIndex == Constants.NO_EXIST_COLUMN)
                {
                    isMemberPush = true;
                }
                else
                {
                    if (MemberRawInfo[MemberIndex, ManualIndex] == "")
                    {
                        isMemberPush = true;
                    }
                }

                if (isMemberPush)
                {
                    string[] values = new string[TotalAttribute];
                    for (int Index = 0; Index < TotalAttribute; Index++)
                    {
                        values[Index] = MemberRawInfo[MemberIndex, Index];
                    }
                    MemberInfo.Add(values);
                }
            }

            MemberLength = MemberInfo.Count;

            if (Log.Config.PreSummary)
            {
                Console.WriteLine("Total Member: " + MemberLength);
            }

            // Data Regulation - Date to Rank (날짜.. 범위 정하기)
            if (DateGroupIndex != Constants.NO_EXIST_COLUMN)
            {
                DateTime[] memberJoinDate = new DateTime[MemberLength];
                for (int MemberIndex = 0; MemberIndex < MemberLength; MemberIndex++)
                {
                    DateTime value = DateTime.Parse(MemberInfo[MemberIndex][DateGroupIndex]);
                    memberJoinDate[MemberIndex] = value;
                }
                Array.Sort(memberJoinDate);

                for (int MemberIndex = 0; MemberIndex < MemberLength; MemberIndex++)
                {
                    DateTime value = DateTime.Parse(MemberInfo[MemberIndex][DateGroupIndex]);

                    MemberInfo[MemberIndex][DateGroupIndex] = Math.Floor(Array.IndexOf(memberJoinDate, value) / (double)(MemberLength / DateGroupRankNumber)).ToString();

                    if (MemberInfo[MemberIndex][DateGroupIndex] == DateGroupRankNumber.ToString())
                    {
                        MemberInfo[MemberIndex][DateGroupIndex] = Math.Floor(Array.IndexOf(memberJoinDate, value) / (double)(MemberLength / DateGroupRankNumber) - 1).ToString();
                    }
                }
            }

            // Data Regulation - Total member considering Manual or Family (짝궁 인원 추리기)
            TotalFamilyMembers = 0;
            if (MorePeopleIndex != Constants.NO_EXIST_COLUMN)
            {
                for (int MemberIndex = 0; MemberIndex < MemberLength; MemberIndex++)
                {
                    if (MemberInfo[MemberIndex][MorePeopleIndex] != "")
                    {
                        TotalFamilyMembers += int.Parse(MemberInfo[MemberIndex][MorePeopleIndex]);
                    }
                }
                AllMemberWithFamily = MemberLength + TotalFamilyMembers;
                if (Log.Config.PreSummary)
                {
                    Console.WriteLine("Total Members with Family - " + AllMemberWithFamily);
                }
            }
            else
            {
                AllMemberWithFamily = MemberLength;
                if (Log.Config.PreSummary)
                {
                    Console.WriteLine("Total Members (no Family) - " + AllMemberWithFamily);
                }
            }

            MemberList.Clear();
            Limits = new Dictionary<string, Limit>();

            ImportantAttribute.Clear();

            MemberData.Clear();
            ElementsAmount = new Dictionary<string, int>();

            AllGroup.Clear();

        }

        private void CheckAttributes()
        {
            for (int AttributeIndex = 0; AttributeIndex < TotalAttribute; AttributeIndex++)
            {
                ErrorCount[AttributeIndex] = 0;
                if (MemberRawInfo[IndexID_Column, AttributeIndex] == "")
                {
                    continue;
                }

                if (Config.Attribute)
                {
                    Console.WriteLine("Attribute - {0}th, {1}", AttributeIndex, MemberRawInfo[ColumnNameRow, AttributeIndex]);
                }

                var One = new Dictionary<string, int>();

                MemberData.Add(One);

                for (int MemberIndex = 0; MemberIndex < MemberLength; MemberIndex++)
                {
                    string Value = MemberInfo[MemberIndex][AttributeIndex];
                    if (One.ContainsKey(Value))
                    {
                        One[Value]++;
                    }
                    else
                    {
                        One[Value] = 1;
                    }

                    string key = KeyMaker(AttributeIndex, Value);
                    if (ElementsAmount.ContainsKey(key))
                    {
                        ElementsAmount[key]++;
                    }
                    else
                    {
                        ElementsAmount[key] = 1;
                    }
                }

                var ForWeight = new List<double>();

                int CountToShow = 0;

                One = One.OrderBy(pair => pair.Key).ToDictionary(pair => pair.Key, pair => pair.Value);

                foreach (var pair in One)
                {
                    CountToShow++;
                    double Ratio = (double)pair.Value / MemberLength;

                    int Max = (int)Math.Ceiling(MaximumInGroup * Ratio);
                    int Min = (int)Math.Floor(MaximumInGroup * Ratio);

                    if (MaximumInGroup * Min >= MemberLength)
                    {
                        Max = Min;
                    }

                    ForWeight.Add(Ratio);

                    if (Config.Attribute)
                    {
                        if (CountToShow < 5)
                        {
                            Console.WriteLine(" ㄴ {0} is {1}%, Min Person: {2}, Max Person: {3}, Total: {4}", pair.Key, (Ratio * 100).ToString("0.00"), Min, Max, pair.Value);
                        }
                        else if (CountToShow == 5)
                        {
                            Console.WriteLine(" ㄴ There are more...");
                        }
                    }

                    string Key = KeyMaker(AttributeIndex, pair.Key);
                    var limit = new Limit(Min, Max, Ratio, 0, 0);
                    Limits[Key] = limit;
                }

                double avg = ForWeight.Average();

                double StandardDeviation = CalculateStandardDeviation(ForWeight.ToArray());
                if (Config.Weights)
                {
                    Console.WriteLine("StandardDeviation: {0}", StandardDeviation);
                }

                foreach (var pair in One)
                {
                    string Key = KeyMaker(AttributeIndex, pair.Key);
                    Limits[Key].StandardDeviation = StandardDeviation;
                }

                ImportantAttribute[AttributeIndex] = StandardDeviation;
            }
        }

        private void ShowReport()
        {
            AllGroup.Sort((a, b) => a.Index - b.Index);
            var ResultMap = new Dictionary<string, string>();

            foreach (var aGroup in AllGroup)
            {
                string debug = "";

                if (Config.GroupMember)
                {
                    debug += "-------------------------------";
                    debug += "Group: " + aGroup.Alias + (aGroup.Lack ? " with Lack" : "") + "\r\n";
                    debug += "Members (" + aGroup.CurrentMemberCount.ToString() + ") info:\r\n";
                }

                if (aGroup.CurrentMemberCount == MaximumInGroup)
                {
                    FitGroup++;
                }
                else if (aGroup.CurrentMemberCount < MaximumInGroup)
                {
                    UnderGroup++;
                }
                else
                {
                    OverGroup++;
                }

                foreach (var memberID in aGroup.Members)
                {
                    if (Config.GroupMember)
                    {
                        debug += "  ㄴ ";
                        for (int Index = 0; Index < TotalAttribute; Index++)
                        {
                            debug += MemberRawInfo[int.Parse(MemberIdToLineNumber[memberID]), Index] + ", ";
                        }
                        debug += "\n";
                    }

                    ResultMap[memberID] = aGroup.Alias;
                }

                aGroup.Totals = new Dictionary<string, int>(aGroup.Totals.OrderBy(pair => pair.Key));

                bool GroupPass = true;

                foreach (var pair in aGroup.Totals)
                {
                    string aKey = pair.Key;
                    int value = pair.Value;

                    if (Limits[aKey].MinPerson <= aGroup.Totals[aKey] && aGroup.Totals[aKey] <= Limits[aKey].MaxPerson)
                    {
                    }
                    else
                    {
                        if (Config.GroupMember)
                        {
                            debug += aKey + " - " + aGroup.Totals[aKey] + " ( " + Limits[aKey].MinPerson + " ~ " + Limits[aKey].MaxPerson + ")\n";
                        }
                        GroupPass = false;

                        int AttributeIndex = int.Parse(aKey.Split()[0]);

                        ErrorCount[AttributeIndex] += 1;
                    }
                }

                if (GroupPass)
                {
                    if (Config.GroupMember)
                    {
                        debug += "Pass!! to make Group\n";
                    }
                    PassGroup++;
                }
                else
                {
                    if (Config.GroupMember)
                    {
                        debug += "Fail!! to make Group" + (aGroup.Lack ? "특이 조직" : "") + "\n";
                    }
                    FailGroup++;
                }

                if (!string.IsNullOrWhiteSpace(debug))
                {
                    Console.WriteLine(debug);
                }

                // Console.WriteLine("ResultMap", ResultMap);
                string sb = "";
                sb += "아래 부터 복사" + "\r\n";
                List<(string userIndex, string value)> objResult = new List<(string userIndex, string value)>();
                for (int Index = 0; Index < MemberRawLength; Index++)
                {
                    string sIndex = "";
                    try
                    {
                        sIndex = MemberRawInfo[Index + ColumnNameRow + 1, 0]; // index
                    }
                    catch
                    {
                        Console.WriteLine("이상해요. 개발자 불러요!");
                        // Environment.Exit(-1);
                    }
                    string value = "";
                    if (ResultMap.ContainsKey(sIndex))
                    {
                        sb += ResultMap[sIndex];
                        value = ResultMap[sIndex];
                    }
                    string UserIndex = sIndex;
                    objResult.Add(MakeStruct(UserIndex, value));

                    sb += "\r\n";
                }
                sb += "위줄까지 복사";

                if (Config.ShowForExcelCopy)
                {
                    Console.WriteLine(sb);
                }
            }
        }

        void EndedUp()
        {
            if (Config.EndUpSummary)
            {
                Console.WriteLine("-----------------------");
                Console.WriteLine("Pass " + PassGroup + ", Fail " + FailGroup + " / All Group " + AllGroup.Count.ToString());

                foreach (var attribute in ImportantAttribute)
                {
                    Console.WriteLine(MemberRawInfo[1, attribute.Key] + "'s Error Count: " + ErrorCount[attribute.Key]);
                }

                Console.WriteLine("Fit groups " + FitGroup);
                Console.WriteLine("Over groups " + OverGroup);
                Console.WriteLine("Under groups " + UnderGroup + " - Expected " + LackGroups);
                Console.WriteLine("-----------------------");
            }

            if (UnderGroup != LackGroups)
            {
                Console.WriteLine("숫자가 맞지 않습니다. 개발자에게 연락하세용!");
            }
        }

        void AssignMemberToGroups()
        {
            if (Config.AllMember)
            {
                foreach (var memberIndex in MemberInfo)
                {
                    string toDebug = "  ㄴ ";
                    for (int index = 0; index < memberIndex.Length; index++)
                    {
                        toDebug += memberIndex[index] + ", ";
                    }
                    Console.WriteLine(toDebug.TrimEnd(',', ' '));
                }
            }

            while (MemberInfo.Count > 0)
            {
                string[] targetMember = MemberInfo[0];

                Group targetGroup = AllGroup[0];
                int minError = int.MaxValue;
                List<Group> sameGroups = new List<Group>();

                for (int index = 0; index < AllGroup.Count; index++)
                {
                    Group aGroup = AllGroup[index];
                    int tempError = GetErrorLevel(aGroup, targetMember);

                    if (tempError < minError)
                    {
                        minError = tempError;
                        sameGroups.Clear();
                        sameGroups.Add(aGroup);
                    }
                    else if (tempError == minError)
                    {
                        sameGroups.Add(aGroup);
                    }
                }

                targetGroup = sameGroups[new Random().Next(0, sameGroups.Count)];

                AddMember(targetGroup, targetMember);

                int memberIndex = MemberInfo.FindIndex(x => x[0] == targetMember[0]);
                MemberInfo.RemoveAt(memberIndex);
            }
        }

        int GetErrorLevel(Group aGroup, string[] targetMember)
        {
            double ReturnValue = 0;

            // Attribute Check
            for (int Index = 0; Index < TotalAttribute; Index++)
            {
                if (MemberRawInfo[0, Index] == "")
                {
                    continue;
                }

                string Key = KeyMaker(Index, targetMember[Index]);

                if (aGroup.Totals.ContainsKey(Key))
                {
                    if (aGroup.Totals[Key] + 1 > Limits[Key].MaxPerson)
                    {
                        ReturnValue += ImportantAttribute[Index] * (aGroup.Totals[Key] + 1 - Limits[Key].MaxPerson) * 10;
                    }
                    else if (aGroup.Totals[Key] + 1 < Limits[Key].MinPerson)
                    {
                        ReturnValue -= ImportantAttribute[Index] * (Limits[Key].MinPerson - (aGroup.Totals[Key] + 1)) * 4;
                    }
                }
                else
                {
                    if (0 != Limits[Key].MinPerson)
                    {
                        ReturnValue -= ImportantAttribute[Index] * 10;
                    }
                    ReturnValue -= ImportantAttribute[Index] * 5;
                }
            }

            // Check Member Counter
            int MoreFamily;
            if (MorePeopleIndex != Constants.NO_EXIST_COLUMN)
            {
                MoreFamily = targetMember[MorePeopleIndex] == "" ? 0 : Convert.ToInt32(targetMember[MorePeopleIndex]);
            }
            else
            {
                MoreFamily = 0;
            }

            if (aGroup.CurrentMemberCount + 1 + MoreFamily > MaximumInGroup)
            {
                ReturnValue = int.MaxValue;
            }
            return (int)ReturnValue;
        }

        void AssignMemberWithFamily()
        {
            if (MorePeopleIndex != Constants.NO_EXIST_COLUMN)
            {
                List<string[]> JoinMember = MemberInfo.Where(x => x[MorePeopleIndex] != "").ToList();
                List<Group> TargetGroup = AllGroup.Where(x => x.Lack == false).ToList();

                int GroupIndex = 0;
                int SearchedMemberIndex;
                for (SearchedMemberIndex = 0; SearchedMemberIndex < JoinMember.Count; SearchedMemberIndex++)
                {
                    AddMember(TargetGroup[GroupIndex], JoinMember[SearchedMemberIndex]);
                    GroupIndex++;
                    if (GroupIndex >= TargetGroup.Count)
                    {
                        Console.WriteLine("짝궁 멤버가 전체 조 대비 많은 듯 합니다.");
                        Console.WriteLine("개발자 연락 바랍니다.");
                        // Environment.Exit(-1);
                    }
                }
            }
        }

        void AddMember(Group one, string[] targetMember)
        {
            bool AddMore = false;

            if (MorePeopleIndex != Constants.NO_EXIST_COLUMN)
            {
                if (targetMember[MorePeopleIndex] != "")
                {
                    AddMore = true;
                }
            }

            if (AddMore)
            {
                one.CurrentMemberCount += int.Parse(targetMember[MorePeopleIndex]) + 1;
            }
            else
            {
                one.CurrentMemberCount += 1;
            }

            one.Members.Add(targetMember[0]);

            for (int Index = 0; Index < TotalAttribute; Index++)
            {
                if (MemberRawInfo[0, Index] == "")
                {
                    continue;
                }

                string Key = KeyMaker(Index, targetMember[Index]);
                if (one.Totals.ContainsKey(Key))
                {
                    one.Totals[Key] += 1;
                }
                else
                {
                    one.Totals[Key] = 1;
                }

                if (MorePeopleIndex != Constants.NO_EXIST_COLUMN)
                {
                    if (targetMember[MorePeopleIndex] != "")
                    {
                        one.Totals[Key] += int.Parse(targetMember[MorePeopleIndex]);
                    }
                }
            }
        }

        public void ShowTotalGroupSummary()
        {
            TotalGroupLength = (int)Math.Ceiling((MemberLength + TotalFamilyMembers) / (double)MaximumInGroup);

            if (Config.PreSummary)
            {
                Console.WriteLine("Total Group is " + TotalGroupLength);
            }

            int LeftMember = AllMemberWithFamily % MaximumInGroup;

            if (LeftMember == 0)
            {
                if (Config.PreSummary)
                {
                    Console.WriteLine("All group(s) have 8 Members");
                }
                LackGroups = 0;
            }
            else
            {
                LackGroups = MaximumInGroup - LeftMember;
                if (Config.PreSummary)
                {
                    Console.WriteLine(LackGroups + " group(s) have 7 Members");
                }
            }

            int LackCount = LackGroups;

            for (int Index = 0; Index < TotalGroupLength; Index++)
            {
                Group NewOne = new Group();
                NewOne.Index = Index;
                NewOne.Alias = (Index + 1).ToString() + " 조";

                if (LackCount > 0)
                {
                    LackCount--;

                    NewOne.Lack = true;
                    NewOne.TargetMemberLength = MaximumInGroup - 1;
                }
                else
                {
                    NewOne.TargetMemberLength = MaximumInGroup;
                }

                AllGroup.Add(NewOne);
            }
        }

        public void ProcessImportantList()
        {
            ImportantAttribute = new Dictionary<int, double>(ImportantAttribute.OrderByDescending(x => x.Value));

            if (Config.Weights)
            {
                Console.WriteLine("중요도 순서...");
            }
            int SmallDiff = 10;
            int Index = (int)Math.Pow(10, SmallDiff);
            if (ImportantAttribute.Count > SmallDiff)
            {
                Console.WriteLine("컬럼이 너무 많아요.. 100개까지만...");
                // process.exit(-1);
            }

            if (Config.Weights)
            {
                Console.WriteLine("------데이터 기반 중요도 순서------");
            }
            foreach (var entry in ImportantAttribute)
            {
                Index /= 10;
                double value = entry.Value * Index;
                value += SmallDiff--;
                if (Config.Weights)
                {
                    Console.WriteLine(MemberRawInfo[1, entry.Key] + " - " + value);
                }
                ImportantAttribute[entry.Key] = value;
            }

            if (Config.Weights)
            {
                Console.WriteLine("------가중치 적용 순서------");
            }
            foreach (var entry in ImportantAttribute)
            {
                double value = entry.Value * (int)Math.Pow(10, int.Parse(MemberRawInfo[0, entry.Key]));
                ImportantAttribute[entry.Key] = value;
            }

            ImportantAttribute = new Dictionary<int, double>(ImportantAttribute.OrderByDescending(x => x.Value));
            if (Config.Weights)
            {
                foreach (var entry in ImportantAttribute)
                {
                    Console.WriteLine(MemberRawInfo[1, entry.Key] + " - " + entry.Value);
                }
            }
        }

        public string KeyMaker(int index, string v)
        {
            return index.ToString() + " " + v;
        }

        public double CalculateStandardDeviation(double[] arr)
        {
            // Creating the mean with Array.Sum
            double mean = arr.Sum() / arr.Length;

            // Assigning (value - mean) ^ 2 to every array item
            arr = arr.Select(k => (k - mean) * (k - mean)).ToArray();

            // Calculating the sum of the updated array
            double sum = arr.Sum();

            // Calculating the variance
            double variance = sum / arr.Length;

            // Returning the Standard deviation
            double standardDeviation = Math.Sqrt(variance);

            return standardDeviation * arr.Length;
        }

        public object GetResult(string[][] sourceData)
        {
            object returnValue;
            errorDetail = "";
            // MemberRawInfo = sourceData;

            if (sourceData[0].Length != Constants.TARGET_COLUMN_COUNT_IN_CONFIG)
            {
                returnValue = Define.HttpStatusCodes.BAD_REQUEST;
            }
            else
            {
                if ("v1" != sourceData[0][0].ToString())
                {
                    Console.WriteLine(sourceData[0][0]);
                    return Define.HttpStatusCodes.HTTP_VERSION_NOT_SUPPORTED;
                }
                MaximumInGroup = Convert.ToInt32(sourceData[0][1]);
                DateGroupRankNumber = Convert.ToInt32(sourceData[0][4]);

                ManualIndex = Constants.NO_EXIST_COLUMN;
                MorePeopleIndex = Constants.NO_EXIST_COLUMN;
                DateGroupIndex = Constants.NO_EXIST_COLUMN;

                int AbsentCount = 0;
                int DateCount = 0;
                int IndexCount = 0;
                int NameCount = 0;
                int NumberCount = 0;
                int PiarCount = 0;
                int TextCount = 0;
                for (int Index = 0; Index < sourceData[1].Length; Index++)
                {
                    if (Config.Attribute)
                    {
                        Console.WriteLine(Index.ToString() + " " + sourceData[1][Index].ToString());
                    }

                    switch (int.Parse(sourceData[1][Index]))
                    {
                        case (int)Define.ColumnTypes.ABSENT:
                            if (AbsentCount != 0)
                            {
                                errorDetail = "ABSENT must be once";
                                return Define.HttpStatusCodes.UNPROCESSABLE_ENTITY;
                            }
                            ManualIndex = Index;
                            AbsentCount++;
                            break;
                        case (int)Define.ColumnTypes.DATE:
                            if (DateCount != 0)
                            {
                                errorDetail = "DATE must be once";
                                return Define.HttpStatusCodes.UNPROCESSABLE_ENTITY;
                            }
                            DateGroupIndex = Index;
                            DateCount++;
                            break;
                        case (int)Define.ColumnTypes.INDEX:
                            if (IndexCount != 0)
                            {
                                errorDetail = "INDEX must be once";
                                IndexID_Column = Index;
                                return Define.HttpStatusCodes.UNPROCESSABLE_ENTITY;
                            }
                            IndexCount++;
                            break;
                        case (int)Define.ColumnTypes.NAME:
                            if (NameCount != 0)
                            {
                                errorDetail = "NAME must be once";
                                return Define.HttpStatusCodes.UNPROCESSABLE_ENTITY;
                            }
                            NameCount++;
                            break;
                        case (int)Define.ColumnTypes.NUMBER:
                            NumberCount++;
                            break;
                        case (int)Define.ColumnTypes.PAIR:
                            if (PiarCount != 0)
                            {
                                errorDetail = "PAIR must be once";
                                return Define.HttpStatusCodes.UNPROCESSABLE_ENTITY;
                            }
                            MorePeopleIndex = Index;
                            PiarCount++;
                            break;
                        case (int)Define.ColumnTypes.TEXT:
                            TextCount++;
                            break;
                        default:
                            return Define.HttpStatusCodes.UNPROCESSABLE_ENTITY;
                    }
                }
                if (IndexCount != 1)
                {
                    errorDetail = "INDEX is needed";
                    return Define.HttpStatusCodes.UNPROCESSABLE_ENTITY;
                }

                // MemberRawInfo = sourceData.Skip(2).ToArray();

                returnValue = Define.HttpStatusCodes.NOT_EXTENDED;

                try
                {
                    returnValue = Define.HttpStatusCodes.OK;
                    Init();
                    ShowTotalGroupSummary();
                    CheckAttributes();
                    ProcessImportantList();
                    AssignMemberWithFamily();
                    AssignMemberToGroups();
                    ShowReport();
                    EndedUp();
                    Console.WriteLine("OK");
                }
                catch (Exception error)
                {
                    Console.WriteLine("Unknown error occur!");
                    returnValue = Define.HttpStatusCodes.EXPECTATION_FAILED;
                }
            }

            return returnValue;

        }
    }
}
