﻿#pragma checksum "..\..\MainWindow.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "21E802C3E16B78EE73B172F9FA8D2181D1CAA6CB221A84D7AF1F00B662253FBA"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Final_Project_DBAS_2023;
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


namespace Final_Project_DBAS_2023 {
    
    
    /// <summary>
    /// MainWindow
    /// </summary>
    public partial class MainWindow : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 41 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button AddPatientButton;
        
        #line default
        #line hidden
        
        
        #line 54 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button AdmitPatientButton;
        
        #line default
        #line hidden
        
        
        #line 67 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button DisplayPatientButton;
        
        #line default
        #line hidden
        
        
        #line 92 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button DischargePatientButton;
        
        #line default
        #line hidden
        
        
        #line 105 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button RoomUtilizationButton;
        
        #line default
        #line hidden
        
        
        #line 122 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label MessageBox;
        
        #line default
        #line hidden
        
        
        #line 147 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button AddPhysicianButton;
        
        #line default
        #line hidden
        
        
        #line 162 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button PhysicianPatientButton;
        
        #line default
        #line hidden
        
        
        #line 178 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button RevenueAnalysisButton;
        
        #line default
        #line hidden
        
        
        #line 193 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button DailyRevenueButton;
        
        #line default
        #line hidden
        
        
        #line 209 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button ExitButton;
        
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
            System.Uri resourceLocater = new System.Uri("/Final_Project_DBAS_2023;component/mainwindow.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\MainWindow.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
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
            
            #line 8 "..\..\MainWindow.xaml"
            ((Final_Project_DBAS_2023.MainWindow)(target)).ContentRendered += new System.EventHandler(this.ContentLoaded_Event);
            
            #line default
            #line hidden
            return;
            case 2:
            this.AddPatientButton = ((System.Windows.Controls.Button)(target));
            
            #line 41 "..\..\MainWindow.xaml"
            this.AddPatientButton.Click += new System.Windows.RoutedEventHandler(this.AddPatient_Click);
            
            #line default
            #line hidden
            return;
            case 3:
            this.AdmitPatientButton = ((System.Windows.Controls.Button)(target));
            
            #line 54 "..\..\MainWindow.xaml"
            this.AdmitPatientButton.Click += new System.Windows.RoutedEventHandler(this.AdmitPatient_Click);
            
            #line default
            #line hidden
            return;
            case 4:
            this.DisplayPatientButton = ((System.Windows.Controls.Button)(target));
            
            #line 67 "..\..\MainWindow.xaml"
            this.DisplayPatientButton.Click += new System.Windows.RoutedEventHandler(this.DisplayPatient_Click);
            
            #line default
            #line hidden
            return;
            case 5:
            this.DischargePatientButton = ((System.Windows.Controls.Button)(target));
            
            #line 92 "..\..\MainWindow.xaml"
            this.DischargePatientButton.Click += new System.Windows.RoutedEventHandler(this.DischargePatient_Click);
            
            #line default
            #line hidden
            return;
            case 6:
            this.RoomUtilizationButton = ((System.Windows.Controls.Button)(target));
            
            #line 105 "..\..\MainWindow.xaml"
            this.RoomUtilizationButton.Click += new System.Windows.RoutedEventHandler(this.RoomUtilization_Click);
            
            #line default
            #line hidden
            return;
            case 7:
            this.MessageBox = ((System.Windows.Controls.Label)(target));
            return;
            case 8:
            this.AddPhysicianButton = ((System.Windows.Controls.Button)(target));
            
            #line 147 "..\..\MainWindow.xaml"
            this.AddPhysicianButton.Click += new System.Windows.RoutedEventHandler(this.AddPhysician_Click);
            
            #line default
            #line hidden
            return;
            case 9:
            this.PhysicianPatientButton = ((System.Windows.Controls.Button)(target));
            
            #line 162 "..\..\MainWindow.xaml"
            this.PhysicianPatientButton.Click += new System.Windows.RoutedEventHandler(this.PhysicianPatient_Click);
            
            #line default
            #line hidden
            return;
            case 10:
            this.RevenueAnalysisButton = ((System.Windows.Controls.Button)(target));
            
            #line 178 "..\..\MainWindow.xaml"
            this.RevenueAnalysisButton.Click += new System.Windows.RoutedEventHandler(this.DailyRevenue_Click);
            
            #line default
            #line hidden
            return;
            case 11:
            this.DailyRevenueButton = ((System.Windows.Controls.Button)(target));
            
            #line 193 "..\..\MainWindow.xaml"
            this.DailyRevenueButton.Click += new System.Windows.RoutedEventHandler(this.RevenueAnalysis_Click);
            
            #line default
            #line hidden
            return;
            case 12:
            this.ExitButton = ((System.Windows.Controls.Button)(target));
            
            #line 209 "..\..\MainWindow.xaml"
            this.ExitButton.Click += new System.Windows.RoutedEventHandler(this.ExitButton_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

