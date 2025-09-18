namespace MetroGid.Core.Exceptions.Truistic;

public class MailInUseException : Exception
{
    public string Mail { get; }

    public MailInUseException(string mail)
        : base($"Почта '{mail}' уже занята")
    {
        Mail = mail;
    }
}