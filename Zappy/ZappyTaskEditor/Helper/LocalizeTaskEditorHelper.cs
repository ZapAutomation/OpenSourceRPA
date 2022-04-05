namespace Zappy.ZappyTaskEditor.Helper
{
    public static class LocalizeTaskEditorHelper
    {
        public static string LanguagePicker(LanguageZappy languageZappy)
        {
            string lang = "en";
            if (languageZappy == LanguageZappy.jp)
            {
                lang = "ja-JP";
            }
                                                            return lang;
        }

                                                                                                                                
                                    }
}