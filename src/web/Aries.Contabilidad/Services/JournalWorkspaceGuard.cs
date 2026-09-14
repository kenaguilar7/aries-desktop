namespace Aries.Contabilidad.Services
{
    public class JournalWorkspaceGuard
    {
        public const string UnbalancedMessage =
            "Este asiento se encuentra descuadrado, cuadre el asiento antes de salir";

        public bool CanLeave { get; private set; } = true;

        public string Message { get; private set; } = UnbalancedMessage;

        public event Action? Changed;

        public void SetCanLeave(bool canLeave, string? message = null)
        {
            CanLeave = canLeave;
            Message = string.IsNullOrWhiteSpace(message) ? UnbalancedMessage : message;
            Changed?.Invoke();
        }

        public void Clear()
        {
            CanLeave = true;
            Message = UnbalancedMessage;
            Changed?.Invoke();
        }
    }
}
