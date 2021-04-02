namespace Mirror.FizzySteam
{
  public interface IServer
  {
    public abstract void ReceiveData();
    public abstract void Send(int connectionId, byte[] data, int channelId);
    public abstract bool Disconnect(int connectionId);
    public abstract void FlushData();
    public abstract string ServerGetClientAddress(int connectionId);
    public abstract void Shutdown();
  }
}