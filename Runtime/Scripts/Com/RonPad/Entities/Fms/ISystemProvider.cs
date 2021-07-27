// *************************************************
// @author : rontian
// @email  : i@ronpad.com
// @date   : 2021/07/21
// *************************************************/

using Com.RonPad.Entities.Core;
namespace Com.RonPad.Entities.Fms
{
    public interface ISystemProvider
    {
        SystemBase GetSystem();

        object Identifier { get; }

        int Priority { get; set; }
    }
}
