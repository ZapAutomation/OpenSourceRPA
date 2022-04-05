namespace Zappy.Decode.Hooks.Keyboard
{
    public interface IUserTextAction
    {
        
        
        
                string ClipboardData { get; set; }
        void Cleanup();
        
    }
}