   M S B A M L   `   `   `  ����D  @DBManager, Version=1.0.0.1, Culture=neutral, PublicKeyToken=nullI)clr-namespace:DBManager.InterfaceElementsDBManager.InterfaceElements  5      O,clr-namespace:DBManager.Excel.Exporting.TabsDBManager.Excel.Exporting.Tabs  5      E'clr-namespace:DBManager.Excel.ExportingDBManager.Excel.Exporting  5      7    1DBManager.Excel.Exporting.Tabs.CLeadReportInfoTab   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35` \RibbonControlsLibrary, Version=4.0.0.11019, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35J FWPFLocalization, Version=0.0.0.0, Culture=neutral, PublicKeyToken=nullH 9http://schemas.microsoft.com/winfx/2006/xaml/presentation      5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      Aviselements)clr-namespace:DBManager.InterfaceElements     5      =tabs,clr-namespace:DBManager.Excel.Exporting.Tabs     5      =	parentWnd'clr-namespace:DBManager.Excel.Exporting     5      .��5      ]� ��. .+�5	      �� $	��Top=�6     �� Orientation$  
Horizontal=�5
      $��5,5,5,5q�5      .+�5      �� ��6   "  WPFLocalization.LocExtension  *resLeadReportXlsPath+�� 5      $	��500��6   $��0,0,5,0q�6   ��6*   �� *	XlsPath+�� 5      -   5        $	��...��5      $��75��5      $��0,0,0,0q�5      $��5q�5      5      5      ]� 5      $	��Top=�6   {�6#   �� *HasLeadReport+.+�5      � $	��Top=�6
   $��5,0,5,5q�6    � RowDefinitions 5      �� 5      �� 5      �� 5      �� 5      �� 5       � ColumnDefinitions 5      �� 5       $	��200 �6   �� 5!      $��50 �6   �� 5"      $	��300 �6   .+�5%      �� $��0��6   $��0��5&   
    �� Padding$ 5q�5'   
   $��0,0,0,5q�5)   
   $
��Wrap=�5*   
   ��5(   
     *resFirstMemberRow+5*   
   +   %DBManager.InterfaceElements.TextBoxEx  5+      $��0��6   $��1��5,      $��0,0,0,5q�5.      $��5q�5/      $��Center=�50         InputCheckType$ PositiveInt=�51      ��5-      �� *FirstMemberRow+   	FieldName 52        *resFirstMemberRow+�� 54      $��1��6   $��0��55   
   $ 5q�56   
   $��0,0,0,5q�58   
   $
��Wrap=�59   
   ��57   
     *resPlaceColumnIndex+59   
     5:      $��1��6   $��1��5;      $��0,0,0,5q�5=      $��5q�5>      $��Center=�5?      $ PositiveInt=�5@      ��5<      �� *PlaceColumnIndex+ 5A        *resPlaceColumnIndex+�� 5C      $��2��6   $��0��5D   
   $ 5q�5E   
   $��0,0,0,5q�5G   
   $
��Wrap=�5H   
   ��5F   
     *resPersonalDataColumnIndex+5H   
     5I      $��2��6   $��1��5J      $��0,0,0,5q�5L      $��5q�5M      $��Center=�5N      $ PositiveInt=�5O      ��5K      �� *PersonalDataColumnIndex+ 5P        *resPersonalDataColumnIndex+�� 5R      $��3��6   $��0��5S   
   $ 5q�5T   
   $��0,0,0,5q�5V   
   $
��Wrap=�5W   
   ��5U   
     *resYearOfBirthColumnIndex+5W   
     5X      $��3��6   $��1��5Y      $��0,0,0,5q�5[      $��5q�5\      $��Center=�5]      $ PositiveInt=�5^      ��5Z      �� *YearOfBirthColumnIndex+ 5_        *resYearOfBirthColumnIndex+�� 5a      $��4��6   $��0��5b   
   $ 5q�5c   
   $��0,0,0,5q�5e   
   $
��Wrap=�5f   
   ��5d   
     *resTeamColumnIndex+5f   
     5g      $��4��6   $��1��5h      $��0,0,0,5q�5j      $��5q�5k      $��Center=�5l      $ PositiveInt=�5m      ��5i      �� *TeamColumnIndex+ 5n        *resTeamColumnIndex+�� 5p      $��0��6   $��2��5q   
   $��10��5r   
   $ 5q�5s   
   $��10,0,0,5q�5u   
   $��Center=�5v   
   $
��Wrap=�5w   
   $
��Bold"�5x   
   ��5t   
     *resNumbersStartWithOne+5x   
   5y      �� 5{      -   5        $��Bottom=�5{      $��Right=�5}      $��5q�5~      $��5q�5      ��5|        *resSaveLeadReportSettings+5�      �� 5�      $��5,0,5,5q�6   $	��200��6(   $	��500��68    �� SelectionMode$ Single=�6f   ��6G   �� *Groups+ �� View 5�      �� 5�       �� AllowsColumnReorder .  .P�5�      �� ��6     *resGroupName+	 �� DisplayMemberBinding	 63   �� *Name+�� 5�      ��6     *resLeadSheetName+
 �� CellTemplate
 5�   	   �� 5�   
   .R�5�      �� $	��250��5�       �� SelectedIndex 5�      �� *LeadSheetIndex+ �� Mode$ TwoWay=���5�      �� *
LeadSheets+ �� RelativeSource ��  �� AncestorType7   1DBManager.Excel.Exporting.Tabs.CLeadReportInfoTab"  5�      5�      5�   	   �� 5�      ��6     *resLeadCompDates+
 5�   	   �� 5�   
   .R�5�      �� $  
Horizontal=�6   .+�5�      �� ��6     *	resFrom+(  "System.Windows.Controls.DatePicker  5�      $	��100��5�         SelectedDateFormat$ Short=�5�         FirstDayOfWeek$ Monday=�5�   5      SelectedDate 5�      �� *	StartDate+5�   5   �� 5�      ��6     *resTo+  5�      $	��100��5�      $ Short=�5�      $ Monday=�5�   5    5�      �� *	EndDate+5�   5   5�      5�      5�   	   5�      5�      5�      5�      5�      