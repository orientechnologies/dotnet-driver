namespace OrientDB.Net.ConnectionProtocols.Binary.Operations.Results
{
    public class DatabaseHandshakeResult
    {
        public bool IsSuccess { get; }

        public DatabaseHandshakeResult(bool isSuccess)
        {
            IsSuccess = isSuccess;
        }
    }
}
