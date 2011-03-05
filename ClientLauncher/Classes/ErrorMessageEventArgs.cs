using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClientLauncher
{
    public class ErrorMessageEventArgs : EventArgs
    {
        private NextStep theNextStep = NextStep.None;

        public NextStep TheNextStep
        {
            get
            {
                return theNextStep;
            }
        }

        public string ErrorMessage
        {
            get;
            private set;
        }

        public EventHandler TheEvent
        {
            get;
            set;
        }

        public ErrorMessageEventArgs(string strErrorMessage, EventHandler handleMe)
        {
            this.ErrorMessage = strErrorMessage;
            this.TheEvent = handleMe;
        }

        public ErrorMessageEventArgs(string strErrorMessage, EventHandler handleMe, NextStep nxtStep)
        {
            this.ErrorMessage = strErrorMessage;
            this.TheEvent = handleMe;
            theNextStep = nxtStep;
        }
    }

    public enum NextStep
    {
        None,
        Patcher
    }
}
