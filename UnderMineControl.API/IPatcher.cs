using System;

namespace UnderMineControl.API
{
    public interface IPatcher
    {
        bool Patch(object caller, Type target, string method, string prefix, string postfix, params Type[] parameters);
    }
}
