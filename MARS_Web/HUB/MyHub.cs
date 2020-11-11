using Microsoft.AspNet.SignalR;

namespace MARS_Web.HUB
{
    public class ExecutionResult : Hub
    {
        public void Hello()
        {
            Clients.All.hello();
        }
        public void Send(string name, string message, string connID)
        {

            //Below Line if want send message to all
            Clients.All.addNewMessageToPage(name, message);

            //      Below line if want to send message to all except requester
            //      Clients.AllExcept(connID).addNewMessageToPage(name, message + connID);

            //      Below line to send message only to requester
            //      Clients.Client(connID).addNewMessageToPage(name, message + " Server time : " + DateTime.Now.ToShortTimeString());
        }
    }
}