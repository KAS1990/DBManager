   M S B A M L   `   `   `  ����D  @DBManager, Version=1.0.0.1, Culture=neutral, PublicKeyToken=null3clr-namespace:DBManager.GlobalDBManager.Global  5   	   I)clr-namespace:DBManager.InterfaceElementsDBManager.InterfaceElements  5      E'clr-namespace:DBManager.Excel.ExportingDBManager.Excel.Exporting  5      1    +DBManager.Excel.Exporting.CExportToExcelWnd   5   !   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35` \RibbonControlsLibrary, Version=4.0.0.11019, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35J FWPFLocalization, Version=0.0.0.0, Culture=neutral, PublicKeyToken=nullH 9http://schemas.microsoft.com/winfx/2006/xaml/presentation      5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   /globclr-namespace:DBManager.Global     5   	   Aviselements)clr-namespace:DBManager.InterfaceElements     5      7wnd'clr-namespace:DBManager.Excel.Exporting     5      0   *DBManager.Global.CNotifyPropertyChangedWnd    Loaded$  CExportToExcelWnd_Loaded��5       � ShowInTaskbar .   � WindowStyle$ 
ToolWindow=�5	       � WindowStartupLocation$ CenterScreen=�5
       � SizeToContent$ WidthAndHeight=�5       � 
ResizeMode$ NoResize=�5       � Title 5      "  WPFLocalization.LocExtension  *resExportToExcelWndTitle+.��5      ]� -   5        $��dpMain��5      ��. .+�5   
   ]� $��Bottom=�6   .+�5      .   (DBManager.InterfaceElements.CancelButton  -   5          Name$ 	btnCancel��5      $��Right=�5      $��5q�5      $��Right=�5      ��5        *	resCancel+5      �� 5      -   5        $��	btnExport��5      $��Right=�5      $��5,5,5,5q�5      $��Right=�5      ��5        *	resExport+5      5      �� 5      $	��Top=�6    �� Orientation$ 
Horizontal=�6$   $��5,5,5,10q�6=   .+�5      �� ��6     *
resXlsPath++   %DBManager.InterfaceElements.TextBoxEx  5       -   5        	  Name$	 
txtXlsPath��5       $	��520��5!   
   $��0,0,5,0q�5"   
   
   InputCheckType$
 NotEmpty=�5$   
      	FieldName 5#   
     *
resXlsPath+5$   
   �� 5%      -   5        $	��...��5%      $��75��5&      $��0,0,0,0q�5'      $��5q�5(      5)      5*      �� 5,      -   5        $��chkShowWbkAfterExport��5-      $��Bottom=�5,      $��5,5,5,0q�5.      ��5/        *resShowWbkAfterExport+ d� 	IsChecked 50      �� *ShowWbkAfterExport+�� 52      -   5        $��
tbctrlTabs��52      $��5,5,5,5q�6!   53      54      