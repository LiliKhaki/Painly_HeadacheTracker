using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Messaging.Messages;

namespace HeadacheTracker.Maui.Messages
{
    public class EntryAddedMessage : ValueChangedMessage<DateTime>
    {
        public EntryAddedMessage(DateTime date) : base(date) { }
    }
}
