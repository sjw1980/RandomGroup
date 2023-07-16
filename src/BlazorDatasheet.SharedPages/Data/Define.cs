using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorDatasheet.SharedPages.Data
{
    public static class Constants
    {
        public static int NO_EXIST_COLUMN { get; } = -1;
        public static int TARGET_COLUMN_COUNT_IN_CONFIG { get; } = 3;
    }

    public static class Log
    {
        public static class Config
        {
            public static bool AllMember { get; set; } = true;
            public static bool GroupMember { get; set; } = true;
            public static bool Attribute { get; set; } = true;
            public static bool Weights { get; set; } = true;
            public static bool PreSummary { get; set; } = true;
            public static bool EndUpSummary { get; set; } = true;
            public static bool ShowForExcelCopy { get; set; } = true;
        }
    }

    internal class Define
    {
        internal const string Result = "결과";
        internal const string PickMeUp = "Picked";
        internal static readonly int AttritubeTypeColumn = 1;
        internal static readonly int WeightColumn = 2;
        internal static readonly int NameColumn = 3;

        public enum ColumnTypes
        {
            INDEX,
            NAME,
            TEXT,
            NUMBER,
            DATE,
            ABSENT,
            PAIR
        }

        public enum HttpStatusCodes
        {
            CONTINUE = 100,
            SWITCHING_PROTOCOLS = 101,
            PROCESSING = 102,
            OK = 200,
            CREATED = 201,
            ACCEPTED = 202,
            NON_AUTHORITATIVE_INFORMATION = 203,
            NO_CONTENT = 204,
            RESET_CONTENT = 205,
            PARTIAL_CONTENT = 206,
            MULTI_STATUS = 207,
            ALREADY_REPORTED = 208,
            IM_USED = 226,
            MULTIPLE_CHOICES = 300,
            MOVED_PERMANENTLY = 301,
            FOUND = 302,
            SEE_OTHER = 303,
            NOT_MODIFIED = 304,
            USE_PROXY = 305,
            TEMPORARY_REDIRECT = 307,
            PERMANENT_REDIRECT = 308,
            BAD_REQUEST = 400,
            UNAUTHORIZED = 401,
            PAYMENT_REQUIRED = 402,
            FORBIDDEN = 403,
            NOT_FOUND = 404,
            METHOD_NOT_ALLOWED = 405,
            NOT_ACCEPTABLE = 406,
            PROXY_AUTHENTICATION_REQUIRED = 407,
            REQUEST_TIMEOUT = 408,
            CONFLICT = 409,
            GONE = 410,
            LENGTH_REQUIRED = 411,
            PRECONDITION_FAILED = 412,
            PAYLOAD_TOO_LARGE = 413,
            URI_TOO_LONG = 414,
            UNSUPPORTED_MEDIA_TYPE = 415,
            RANGE_NOT_SATISFIABLE = 416,
            EXPECTATION_FAILED = 417,
            IM_A_TEAPOT = 418,
            MISDIRECTED_REQUEST = 421,
            UNPROCESSABLE_ENTITY = 422,
            LOCKED = 423,
            FAILED_DEPENDENCY = 424,
            TOO_EARLY = 425,
            UPGRADE_REQUIRED = 426,
            PRECONDITION_REQUIRED = 428,
            TOO_MANY_REQUESTS = 429,
            REQUEST_HEADER_FIELDS_TOO_LARGE = 431,
            UNAVAILABLE_FOR_LEGAL_REASONS = 451,
            INTERNAL_SERVER_ERROR = 500,
            NOT_IMPLEMENTED = 501,
            BAD_GATEWAY = 502,
            SERVICE_UNAVAILABLE = 503,
            GATEWAY_TIMEOUT = 504,
            HTTP_VERSION_NOT_SUPPORTED = 505,
            VARIANT_ALSO_NEGOTIATES = 506,
            INSUFFICIENT_STORAGE = 507,
            LOOP_DETECTED = 508,
            NOT_EXTENDED = 510,
            NETWORK_AUTHENTICATION_REQUIRED = 511
        }
    }

    public static class SampleRawInfo
    {
        public static string[][] Data = new string[][]
        {
            new string[] { "v1", "2", "4" },
            new string[] { "INDEX", "TEXT", "TEXT", "TEXT", "TEXT", "TEXT", "TEXT", "TEXT", "TEXT", "DATE", "ABSENT", "PAIR" },
            new string[] { "", "", "1", "1", "1", "1", "", "", "10", "1", "", "" },
            new string[]
            {
                "No.",
                "조직 구분",
                "그룹",
                "실",
                "팀",
                "역할",
                "성명",
                "별명",
                "성별",
                "입사일자",
                "수동(미참석)",
                "짝꿍"
            },
            new string[]
            {
                "1",
                "조직A",
                "조직A",
                "조직A",
                "조직A",
                "대표이사",
                "이건희",
                "내가짱",
                "남",
                "2011-01-01",
                "",
                ""
            },
            new string[]
            {
                "2",
                "조직A",
                "데이터그룹",
                "데이터그룹",
                "데이터개발팀",
                "팀리드",
                "정주영",
                "맨발청춘",
                "남",
                "2013-01-21",
                "",
                ""
            },
            new string[]
            {
                "3",
                "조직A",
                "DX실",
                "DX실",
                "DX실",
                "실리드",
                "관우",
                "장비형",
                "남",
                "2014-02-01",
                "",
                ""
            },
            new string[]
            {
                "4",
                "조직A",
                "경영관리그룹",
                "재무회계실",
                "재무팀",
                "팀리드",
                "김기리",
                "개콘짱",
                "남",
                "2014-04-28",
                "",
                ""
            }
        };
    }
}
