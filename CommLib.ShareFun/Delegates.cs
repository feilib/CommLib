using System;


namespace CommLib.ShareFun
{
    public delegate void MyDelegateVoid();
    public delegate void MyDelegateString(string val);
    public delegate void MyDelegateInt(int val);
    public delegate void MyDelegateDouble(double val);
    public delegate void MyDelegateBool(bool val);
    public delegate void MyDelegateObj(object val);
    public delegate void MyDelegateEvent(object sender, EventArgs e);

    public class Delegates
    {
    }
}
