using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PJTC.Enums;

namespace PJTC.Scripts
{
    public interface MessageSender
    {
        public void SendMessage<T>(CSMRequest.Type type, T body, bool needAck);
    }
}
