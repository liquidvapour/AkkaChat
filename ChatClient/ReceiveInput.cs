namespace ChatClient
{
    public class ReceiveInput
    {
        public ReceiveInput(string input)
        {
            Input = input;
        }

        public string Input { get; private set; }
    }
}