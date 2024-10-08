namespace OrchidPharmed.Core.HiringTask.API.Structure
{
    public class APIResponse
    {
        public object? ResultObject { get; set; }
        public string? ResultText { get; set; }
        public bool TokenExpired { get; set; }
        public bool Refused { get; set; }
        public string[]? ExtraCommands { get; set; }
        public int ResultCode { get; set; }
        public bool ErrorFlag { get; set; }
        public string? ResultTextAlias { get; set; }
        public bool ProblemFlag => TokenExpired || Refused || ErrorFlag;
    }
}
