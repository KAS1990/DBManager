   M S B A M L   `   `   `  ���� O  Kmscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e0891&clr-namespace:System;assembly=mscorlibSystem  D @DBManager, Version=1.0.0.1, Culture=neutral, PublicKeyToken=null3clr-namespace:DBManager.GlobalDBManager.Global I)clr-namespace:DBManager.Global.ConvertersDBManager.Global.Converters ` \RibbonControlsLibrary, Version=4.0.0.11019, Culture=neutral, PublicKeyToken=31bf3856ad364e35tNclr-namespace:Microsoft.Windows.Controls.Ribbon;assembly=RibbonControlsLibrary!Microsoft.Windows.Controls.Ribbon %clr-namespace:DBManager	DBManager =#clr-namespace:DBManager.RightPanelsDBManager.RightPanels I)clr-namespace:DBManager.InterfaceElementsDBManager.InterfaceElements U/clr-namespace:DBManager.RoundMembers.Converters!DBManager.RoundMembers.Converters    DBManager.MainWindow   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35J FWPFLocalization, Version=0.0.0.0, Culture=neutral, PublicKeyToken=nullH 9http://schemas.microsoft.com/winfx/2006/xaml/presentation      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     6sys&clr-namespace:System;assembly=mscorlib     /globclr-namespace:DBManager.Global     <gconvs)clr-namespace:DBManager.Global.Converters     aribbonNclr-namespace:Microsoft.Windows.Controls.Ribbon;assembly=RibbonControlsLibrary     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     )localclr-namespace:DBManager     5rpnls#clr-namespace:DBManager.RightPanels     @
localctrls)clr-namespace:DBManager.InterfaceElements     Crmconvs/clr-namespace:DBManager.RoundMembers.Converters     -   0  *DBManager.Global.CNotifyPropertyChangedWnd   Name$  
mainWindow��0  *DBManager.Global.CNotifyPropertyChangedWnd   Loaded$ Window_Loaded�� � 
ResizeMode$ CanResizeWithGrip=� � Icon$1 +/DBManager;component/Icons/AppIcon_tune.ico�� � WindowState$ 	Maximized=� � 	Resources  �� MergedDictionaries �� Source   vsrcCurrentRoundMembers  vsrcCurrentRoundMembers2��  �� $ RibbonStyles.xaml?��� $< 6RoundResultsControl\RoundResultsDataGridTemplates.xaml?�%   &	        &	      �� ��  J� CommandBindings )  #System.Windows.Input.CommandBinding  -     DBManager.MainWindow	   cmdOpenFilterPopup
   Command#
 Z	 .��]� -   $��dpMain����. .+�.  (Microsoft.Windows.Controls.Ribbon.Ribbon  -     Name$ Ribbon��$	��Top=� �� SelectedIndex$ 0��   QuickAccessToolBar @  :Microsoft.Windows.Controls.Ribbon.RibbonQuickAccessToolBar   J� 	AllowDrop .   4  .Microsoft.Windows.Controls.Ribbon.RibbonButton �� 
TargetType �� Property �� ValueA  ;Microsoft.Windows.Controls.Ribbon.RibbonApplicationMenuItem6	  0Microsoft.Windows.Controls.Ribbon.RibbonMenuItem��%�   '        '  D     �� "  .��� * �@ "CanAddToQuickAccessToolBarDirectly .  �� "  .��� * �@	 "CanAddToQuickAccessToolBarDirectly .     HelpPaneContent ��  �� Orientation$ 
Horizontal=�.+���  �� Padding$ 5,1,5,1q�
���  ����"
  WPFLocalization.LocExtension
  *resGodsMode+y��� *
GodsMode+ �� 	Converter A  ;DBManager.Global.Converters.BoolToVisibilityMarkupConverter     UnvisibleValue$ Hidden=��� -   $��txtblkErrLogItemChanged��$ 5,1,5,1q�
���  ��$y�Hidden=���
  *resErrLogItemChanged+  -      SmallImageSource$1 +/DBManager;component/RibbonIcons/Help_6.png��   ApplicationMenu =  7Microsoft.Windows.Controls.Ribbon.RibbonApplicationMenu  .@�  -     Name$ ramiExportToXls�� 	  ImageSource$3 -/DBManager;component/Icons/KEY_EXPORT_XLS.png�� / +RibbonApplicationMenuItemWithGoodImgScaling#��[ ��
  *resExportToXls+{��� *ExportToXlsEnabled+  -   $ ramiExportFromXls��$- '/DBManager;component/Icons/new_comp.png��#��[ ��
  *resCreateCompetition+{��� *CreateCompetitionEnabled+.@�1  +Microsoft.Windows.Controls.Ribbon.RibbonTab  -	     Name$ 
ribtabMain����
  *	resMain+.@�3  -Microsoft.Windows.Controls.Ribbon.RibbonGroup  ��
  *resTableUpdating+.@�  -
   $��rbtnStartStop��  StartStopRibbonButtonStyle#��[  * &RibbonButtonWithGoodImgScalingTemplate#��[    Label 
  *resAutoupdating+  -      LargeImageSource$ /RibbonIcons/Refresh.png��#��[  
  *
resRefresh+{��� *RefreshEnabled+ � ToolTip 
  *resRefreshToolTip+  -   $&  /RibbonIcons/SyncDBWithFiles.png��#��[  
  *resSyncDBWithFiles+{��� *SyncDBWithFilesEnabled+ 
  *resSyncDBWithFilesToolTip+  -   $&  /RibbonIcons/SyncDBWithFiles.png�� 
  *resDBToGrid+{��� *DBToGridEnabled+ 
  *resDBToGridToolTip+  ��
  *resTableContent+.@�8  2Microsoft.Windows.Controls.Ribbon.RibbonMenuButton  -   $��
rmbtnGroup��   Label 
  *resSelectGroup+{��� *CurrentGroups.Count+ 5  /DBManager.CollectionsCountToBoolMarkupConverter  ���� *CurrentGroups+   �� Path��%�   '	        �� " 	 .���  � �� �� $(  "Value.Value.AgeGroup.FullGroupName���  �@m�Command �� $  Value.Command�  -   $��
rmbtnRound�� 
  *resSelectRound+{��� *CurrentRounds.Count+   ���� *CurrentRounds+ ��%v   '	        �� " 	 .���  � �� �� $  Value.Value.Name���  �@m�Command �� $  Value.Command�  -   $��rbtnOpenWorkbook��$ /RibbonIcons/Open.png�� 
  *resOpenWorkbook+{��� *	IsEnabled+! �� ElementName$! 
rmbtnRound�� 
  *resOpenWorkbookToolTip+6  0Microsoft.Windows.Controls.Ribbon.RibbonCheckBox  -   $��rchkShowGroupHead��" d� 	IsChecked$
" TrueY�#   Label# 
  *resShowGroupHead+  -   $��rchkAutoscrollEnabled��$
" TrueY�# 
  *resAutoscrollEnabled+  ��
  *resPublishing+{��� *PublishingEnabled+.@�  -   $) #/RibbonIcons/PublishingSettings.png�� 
  *resPublishingSettings+  -   $) #/RibbonIcons/server-from-client.png�� 
  *resPublishResults+  -   $!��rbtnPublishResultsLogWindow��$# /RibbonIcons/CreateReport.png�� 
  *resPublishingLog+  -   $��rchkAutoPublishing��# 
  *resAutoPublishing+  -   $ ribtabConfig����
  *	resConfig+.@�  $�� ��.@�  -   $ RibbonIcons/settings.png��#��[  
  *resSettings+{��� *SettingsEnabled+  -   $��rbtnLogWindow��$" RibbonIcons/CreateReport.png�� 
  *resLogWindow+{��� *LogWindowEnabled+  -   $��rbtnShowRemoteControlWnd��$* $RibbonIcons/if_teamviewer_100417.png��#��[  
  *resShowRemoteControlWnd+  -   $��rbtnFalsestartRules�� 
  *resFalsestartRules+{��� *FalsestartRulesEnabled+  # 
  *resGodsMode+" �� *
GodsMode+  ��
  *	resGrades+{��� *CalcGradesEnabled+.@�  -    
  *resCalcGrades+  -   $��mbtnHighlightGrades������ *HighlightTypes+ ��%e   '	        �� " 	 .���  � �� �� $  Value���  �@m�Command �� $  Command�� $ � RowDefinitions$ �� $
��auto ��� $
��auto ��� $
��auto ��� �� $
��auto �% � ColumnDefinitions% �� $��50��.+�� -   $��grdGroupHead��$��0��$��0�� ��%0   '��       �� " ��.���  � ��$ Arial)�$ �� $
��auto ��� $
��auto ��� $
��auto ��� $
��auto ��� $
��auto �% �� �� .+��� -   $��lblCompName��$��0��$��0��$��10��$��Center=�$��20(�$
��Bold"��� -    $��lblLocation��$��1��$��0��$
��Left=�$��14(���
  *resSmolensk+�� -!   $��lblRoundDate��$��1��$��1��$��Right=�$��14(��� -"   $��lblRoundName��$��2��$��0��$��10��$��Center=�$��20(��� $ 
Horizontal=�$��3��$��0��$��10��   	Row6Style& �� BasedOn' �� Triggers( �� Binding) �� Value��%r   &	       7���� " ��8&   ' �� ( �� *	Content+$! lblRow6��) Z� .a���  � y�$ 	Collapsed=�.+��� $��0,0,0,0q�$��14(���
  *resMainJudge+�� -#   $��lblMainJudge��$��0,0,10,0q�$��14(��� $��0,0,0,0q�$��14(���
  *resMainSecretary+�� -$   $��lblMainSecretary��$��0,0,10,0q�$��14(��� $��14(�#��[ ��
  *	resRow6+�� -%   $��lblRow6��$��0,0,0,0q�$��14(�#��[ �� -&   $%��scrlvwrAdditionalDataGridHeader��$��2��$��Arial)�$��14(�$��Disabled=�$��Hidden=�.���� -'   $ ��spAdditionalDataGridHeader��$ 
Horizontal=�.+��� -(   $��lblLeftDummy��$��21,0,0,0q�$��0q��� -)   $��lbRightDummy��$��0,0,17,0q�$��0q�� -*   $��grdRoundMembersHost��$��3��% �� $	��100���� $
��auto ��� .+�&   System.Windows.Controls.DataGrid  -+   $��dgrdRoundMembers��$��0��*   SelectionMode$* Single=�+   AutoGenerateColumns+ .  ,   EnableRowVirtualization, . - $� VirtualizationMode$- Standard=�. $� IsVirtualizing. . /   CanUserReorderColumns/ .  0   CanUserResizeRows0 .  1   CanUserSortColumns1 .  2   CanUserAddRows2 .  3   CanUserDeleteRows3 .  4   RowHeaderWidth$4 1Q�5   
IsReadOnly5 . ��. $
��Auto=�$
��Auto=�6   MinColumnWidth$6 2Q�$��14(�$��Arial)�7   ColumnHeaderHeight$7 30Q����� 8 �� Source#8 [  9   HorizontalGridLinesBrush9 �� *$"GridLinesFontStyle.BackgroundColor+ B  <DBManager.Global.Converters.ColorToSolidBrushMarkupConverter  :   VerticalGridLinesBrush: �� *$"GridLinesFontStyle.BackgroundColor+   �� -,   $��grdspltrRoundMembers��$��1��$��5��; �� ResizeDirection$; Columns=�$��Stretch=�$��Stretch=�  --   $��dgrdRoundMembers2��$��2��$* Single=�+ .  , . $- Standard=�. . / .  0 .  1 .  2 .  3 .  $4 1Q�5 . ��. $
��Auto=�$
��Auto=�$6 2Q�$��14(�$��Arial)�$7 30Q����� #8 [ 9 �� *$"GridLinesFontStyle.BackgroundColor+   : �� *$"GridLinesFontStyle.BackgroundColor+   .  (DBManager.RightPanels.CRightPanelControl  -.   <  Name$< 
RightPanel��$��4��$��0��$��Stretch=�$��Stretch=�