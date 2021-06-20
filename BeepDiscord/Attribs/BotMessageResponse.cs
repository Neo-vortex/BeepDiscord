namespace BeepDiscord.Attribs
{
    [System.AttributeUsage(System.AttributeTargets.Method )]  
    public class BotMessageResponse : System.Attribute  
    {  
        public string IncommingMessageBody { get; set; }

        

        public BotMessageResponse(string IncommingMessageBody)  
        {  
            this.IncommingMessageBody = IncommingMessageBody;

        }  
    }  
}