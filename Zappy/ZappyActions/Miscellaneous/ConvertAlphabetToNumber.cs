using System;
using System.ComponentModel;
using Zappy.SharedInterface;
using Zappy.SharedInterface.Helper;

namespace Zappy.ZappyActions.Miscellaneous
{
                [Description("Gets Number From Enter Character")]
    public class ConvertAlphabetToNumber : TemplateAction
    {
                                [Category("Input")]
        [Description("Input Character for to get the Number")]
        public DynamicProperty<char> InputCharacter { get; set; }

                                [Category("Output")]
        [Description("Gets CharacterNumber from InputCharacter")]
        public int CharacterNumber { get; set; }
                                                                public override void Invoke(IZappyExecutionContext context, ZappyTaskActionInvoker actionInvoker)

        {
            if (char.IsLetter(InputCharacter))
            {
                CharacterNumber = char.ToUpper(InputCharacter) - 64;            }
            else
            {
                throw new Exception("InputCharacter is not a letter");
            }

        }

        public override string AuditInfo()
        {
            return base.AuditInfo() + " InputCharacter: " + InputCharacter + " Output Integer Value: " + CharacterNumber;
        }
    }
}