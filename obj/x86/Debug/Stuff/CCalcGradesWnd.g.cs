﻿#pragma checksum "..\..\..\..\Stuff\CCalcGradesWnd.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "CE3B89C7656F52535940ACBF31E7872059E03A7C"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using DBManager.Global;
using Microsoft.Windows.Controls;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;
using WPFLocalization;


namespace DBManager.Stuff {
    
    
    /// <summary>
    /// CCalcGradesWnd
    /// </summary>
    public partial class CCalcGradesWnd : DBManager.Global.CNotifyPropertyChangedWnd, System.Windows.Markup.IComponentConnector {
        
        
        #line 13 "..\..\..\..\Stuff\CCalcGradesWnd.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.CheckBox chkSelectAll;
        
        #line default
        #line hidden
        
        
        #line 37 "..\..\..\..\Stuff\CCalcGradesWnd.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Grid grdCalcGradesResults;
        
        #line default
        #line hidden
        
        
        #line 68 "..\..\..\..\Stuff\CCalcGradesWnd.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox cmbResultGradeCalcMethod;
        
        #line default
        #line hidden
        
        
        #line 75 "..\..\..\..\Stuff\CCalcGradesWnd.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.CheckBox chkOnly75PercentForCalcGrades;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/DBManager;component/stuff/ccalcgradeswnd.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\Stuff\CCalcGradesWnd.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal System.Delegate _CreateDelegate(System.Type delegateType, string handler) {
            return System.Delegate.CreateDelegate(delegateType, this, handler);
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.chkSelectAll = ((System.Windows.Controls.CheckBox)(target));
            
            #line 16 "..\..\..\..\Stuff\CCalcGradesWnd.xaml"
            this.chkSelectAll.Click += new System.Windows.RoutedEventHandler(this.chkSelectAll_Click);
            
            #line default
            #line hidden
            return;
            case 2:
            
            #line 22 "..\..\..\..\Stuff\CCalcGradesWnd.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.btnRemoveGrades_Click);
            
            #line default
            #line hidden
            return;
            case 3:
            
            #line 27 "..\..\..\..\Stuff\CCalcGradesWnd.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.btnCalcPlaces_Click);
            
            #line default
            #line hidden
            return;
            case 4:
            
            #line 32 "..\..\..\..\Stuff\CCalcGradesWnd.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.btnSetGrades_Click);
            
            #line default
            #line hidden
            return;
            case 5:
            this.grdCalcGradesResults = ((System.Windows.Controls.Grid)(target));
            return;
            case 6:
            this.cmbResultGradeCalcMethod = ((System.Windows.Controls.ComboBox)(target));
            return;
            case 7:
            this.chkOnly75PercentForCalcGrades = ((System.Windows.Controls.CheckBox)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

