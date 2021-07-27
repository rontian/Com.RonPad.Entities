// *************************************************
// @author : rontian
// @email  : i@ronpad.com
// @date   : 2021/07/21
// *************************************************/

namespace Com.RonPad.Entities.Fms
{
    public interface ISystemProvider
    {
        Core.System GetSystem();

        object Identifier { get; }

        int Priority { get; set; }
    }
}
