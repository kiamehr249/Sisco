using Microsoft.AspNetCore.Mvc;
using NiksoftCore.ViewModel;
using System.Collections.Generic;

namespace NiksoftCore.MiddlController.Middles
{
    public class NikController : Controller
    {
        public List<NikMessage> Messages;

        public NikController()
        {
            Messages = new List<NikMessage>();
        }

        public void AddError(string message, string lang = "fa")
        {
            Messages.Add(new NikMessage { 
                Message = message,
                Language = lang,
                Type = MessageType.Error
            });
        }

        public void AddSuccess(string message, string lang = "fa")
        {
            Messages.Add(new NikMessage
            {
                Message = message,
                Language = lang,
                Type = MessageType.Success
            });
        }


    }
}
