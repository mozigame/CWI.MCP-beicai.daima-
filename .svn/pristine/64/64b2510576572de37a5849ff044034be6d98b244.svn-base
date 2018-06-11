using System;
using System.Linq;
using System.Text;
using Sodao.FastSocket.SocketBase;

namespace Sodao.FastSocket.Server.Protocol
{
    /// <summary>
    /// Json行协议(Json协议)
    /// </summary>
    public sealed class McpProtocol : IProtocol<Command.McpCommandInfo>
    {
        static private readonly string[] SPLITER = new string[] { "|" };

        #region IProtocol Members
        /// <summary>
        /// find command
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="buffer"></param>
        /// <param name="maxMessageSize"></param>
        /// <param name="readlength"></param>
        /// <returns></returns>
        /// <exception cref="BadProtocolException">bad command line protocol</exception>
        public Command.McpCommandInfo FindCommandInfo(IConnection connection, ArraySegment<byte> buffer,
            int maxMessageSize, out int readlength)
        {
            if (buffer.Count < 2) { readlength = 0; return null; }

            var payload = buffer.Array;

            //查找 } 标记符
            for (int i = buffer.Offset, len = buffer.Offset + buffer.Count; i < len; i++)
            {
                if (payload[i] == 125)
                {
                    readlength = i + 1 - buffer.Offset;

                    if (readlength == 2) return null;
                    if (readlength > maxMessageSize) throw new BadProtocolException("message is too long");

                    string data = Encoding.UTF8.GetString(payload, buffer.Offset, readlength);
                    if (string.IsNullOrEmpty(data)) 
                        return null;

                    return new Command.McpCommandInfo("TcpDataHandel", data);
                }
            }
            readlength = 0;
            return null;
        }
        #endregion
    }
}