using IronPython.Hosting;
using IronPython.Runtime.Binding;
//using IronPython.Hosting;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace PythonRange_Crash
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }


        private async Task ShowMessage(String message)
        {
            MessageDialog dialog = new MessageDialog(message);
            await dialog.ShowAsync();
        }

        private async void Button_Call_Site_KeyValuePair(object sender, RoutedEventArgs e)
        {
            CallSite<Func<CallSite, Object, KeyValuePair<IEnumerator, IDisposable>>> cs 
                = CallSite<Func<CallSite, Object, KeyValuePair<IEnumerator, IDisposable>>>.Create(new KeyValuePairCallSiteBinder());
            var r = cs.Target(cs, new object());
            await ShowMessage(r.GetType().FullName);
        }

        private async void Button_Call_Site_MyKeyValuePair(object sender, RoutedEventArgs e)
        {
            CallSite<Func<CallSite, Object, MyKeyValuePair<IEnumerator, IDisposable>>> cs 
                = CallSite<Func<CallSite, Object, MyKeyValuePair<IEnumerator, IDisposable>>>.Create(new MyKeyValuePairCallSiteBinder());
            var r = cs.Target(cs, new object());
            await ShowMessage(r.GetType().FullName);
        }

        private async void Button_Python_Range_Click(object sender, RoutedEventArgs e)
        {
            string s = "for i in range(0,10):\n\tprint(i)\n";
            var engine = Python.CreateEngine();
            engine.Execute(s);
            await ShowMessage("Python run finish");
        }
    }

    #region KeyValuePairCallSiteBinder 
    public class KeyValuePairCallSiteBinder : DynamicMetaObjectBinder
    {
        public override DynamicMetaObject Bind(DynamicMetaObject target, DynamicMetaObject[] args)
        {
            throw new NotImplementedException();
        }

        public override T BindDelegate<T>(CallSite<T> site, object[] args)
        {
            T t = (T)(object)new Func<CallSite, object, KeyValuePair<IEnumerator, IDisposable>>(GetListEnumerator);
            return t;
        }

        private KeyValuePair<IEnumerator, IDisposable> GetListEnumerator(CallSite site, Object value)
        {
            return new KeyValuePair<IEnumerator, IDisposable>(new List<Object> { value }.GetEnumerator(), null);
        }
    }
    #endregion


    #region MyKeyValuePairCallSiteBinder 
    public class MyKeyValuePairCallSiteBinder : DynamicMetaObjectBinder
    {
        public override DynamicMetaObject Bind(DynamicMetaObject target, DynamicMetaObject[] args)
        {
            throw new NotImplementedException();
        }

        public override T BindDelegate<T>(CallSite<T> site, object[] args)
        {
            T t = (T)(object)new Func<CallSite, object, MyKeyValuePair<IEnumerator, IDisposable>>(GetListEnumerator);
            return t;
        }

        private MyKeyValuePair<IEnumerator, IDisposable> GetListEnumerator(CallSite site, Object value)
        {
            return new MyKeyValuePair<IEnumerator, IDisposable>();
        }
    }

    #endregion

}



