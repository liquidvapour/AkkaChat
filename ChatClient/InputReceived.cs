namespace ChatClient
{
    internal class InputReceived
    {
        public InputReceived(string input)
        {
            Input = input;
        }

        public string Input { get; private set; }
    }
}