using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace DoodleCycle
{
  public class ItemViewModel : INotifyPropertyChanged
  {
    /// <summary>
    /// Sample ViewModel property; this property is used in the view to display its value using a Binding.
    /// </summary>
    /// <returns></returns>
    public string LineOne
    {
      get;
      set;
    }

    /// <summary>
    /// Sample ViewModel property; this property is used in the view to display its value using a Binding.
    /// </summary>
    /// <returns></returns>
    public string LineTwo
    {
      get;
      set;
    }

    /// <summary>
    /// Sample ViewModel property; this property is used in the view to display its value using a Binding.
    /// </summary>
    /// <returns></returns>
    public string LineThree
    { get; set; }

    public event PropertyChangedEventHandler PropertyChanged;
  }
}