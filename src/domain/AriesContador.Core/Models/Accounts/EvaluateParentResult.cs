namespace AriesContador.Core.Models.Accounts
{
    public class EvaluateParentResult
    {
        public bool CanProceed { get; set; }

        public string Message { get; set; }

        public static EvaluateParentResult Allow()
        {
            return new EvaluateParentResult { CanProceed = true, Message = "" };
        }

        public static EvaluateParentResult Warn(string message)
        {
            return new EvaluateParentResult { CanProceed = false, Message = message ?? "" };
        }
    }
}
