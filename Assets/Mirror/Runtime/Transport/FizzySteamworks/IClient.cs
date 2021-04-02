namespace Mirror.FizzySteam
{
  public interface IClient
  {
    public abstract bool Connected { get; }
    public abstract bool Error { get; }


    public abstract void ReceiveData();
    public abstract void Disconnect();
    public abstract void FlushData();
    public abstract void Send(byte[] data, int channelId);
  }
}