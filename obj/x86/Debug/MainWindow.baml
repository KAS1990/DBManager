   M S B A M L   `   `   `  ����O  Kmscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e0891&clr-namespace:System;assembly=mscorlibSystem  5   	   D @DBManager, Version=1.0.0.1, Culture=neutral, PublicKeyToken=null3clr-namespace:DBManager.GlobalDBManager.Global 5   	   I)clr-namespace:DBManager.Global.ConvertersDBManager.Global.Converters 5   	   ` \RibbonControlsLibrary, Version=4.0.0.11019, Culture=neutral, PublicKeyToken=31bf3856ad364e35tNclr-namespace:Microsoft.Windows.Controls.Ribbon;assembly=RibbonControlsLibrary!Microsoft.Windows.Controls.Ribbon 5   	   %clr-namespace:DBManager	DBManager 5
   	   =#clr-namespace:DBManager.RightPanelsDBManager.RightPanels 5   	   I)clr-namespace:DBManager.InterfaceElementsDBManager.InterfaceElements 5   	   U/clr-namespace:DBManager.RoundMembers.Converters!DBManager.RoundMembers.Converters 5   	      DBManager.MainWindow   5   !   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35J FWPFLocalization, Version=0.0.0.0, Culture=neutral, PublicKeyToken=nullH 9http://schemas.microsoft.com/winfx/2006/xaml/presentation      5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   6sys&clr-namespace:System;assembly=mscorlib     5   	   /globclr-namespace:DBManager.Global     5   	   <gconvs)clr-namespace:DBManager.Global.Converters     5   	   aribbonNclr-namespace:Microsoft.Windows.Controls.Ribbon;assembly=RibbonControlsLibrary     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5	   	   )localclr-namespace:DBManager     5
   	   5rpnls#clr-namespace:DBManager.RightPanels     5   	   @
localctrls)clr-namespace:DBManager.InterfaceElements     5   	   Crmconvs/clr-namespace:DBManager.RoundMembers.Converters     5   	   -   5        0  *DBManager.Global.CNotifyPropertyChangedWnd   Name$  
mainWindow��5   	   0  *DBManager.Global.CNotifyPropertyChangedWnd   Loaded$ Window_Loaded��5   	    � 
ResizeMode$ CanResizeWithGrip=�5   	    � Icon$1 +/DBManager;component/Icons/AppIcon_tune.ico��5   	    � WindowState$ 	Maximized=�5   	    � 	Resources 5       �� MergedDictionaries �� Source   vsrcCurrentRoundMembers  vsrcCurrentRoundMembers2�� 5       5      �� 5      $ RibbonStyles.xaml?�6   �� 5      $< 6RoundResultsControl\RoundResultsDataGridTemplates.xaml?�6   %:   &	        &	      �� 5      6   �� 5      6   5       J� CommandBindings 5      )  #System.Windows.Input.CommandBinding  5!      -   5          DBManager.MainWindow	   cmdOpenFilterPopup
   Command#
 Z	 5!      5#      .��5&      ]� -   5        $��dpMain��5'      ��. .+�5)      .  (Microsoft.Windows.Controls.Ribbon.Ribbon  -   5          Name$ Ribbon��5)      $	��Top=�5*   	    �� SelectedIndex$ 0��5+   	      QuickAccessToolBar 5,      @  :Microsoft.Windows.Controls.Ribbon.RibbonQuickAccessToolBar  5-       J� 	AllowDrop .  6&    50      4  .Microsoft.Windows.Controls.Ribbon.RibbonButton �� 
TargetType �� Property �� ValueA  ;Microsoft.Windows.Controls.Ribbon.RibbonApplicationMenuItem6	  0Microsoft.Windows.Controls.Ribbon.RibbonMenuItem��%�   '        '  m     �� 51      "  6   .�52      �� * �@ "CanAddToQuickAccessToolBarDirectly .  53      54      �� 55      "  6   .�56      �� * �@	 "CanAddToQuickAccessToolBarDirectly .  57      58      59         HelpPaneContent 5;      �� 5<       �� Orientation$ 
Horizontal=�6   .+�5=      ��  �� Padding$ 5,1,5,1q�5>      
���  ����5=      "
  WPFLocalization.LocExtension
  *resGodsMode+y�5@      �� *
GodsMode+ �� 	Converter A  ;DBManager.Global.Converters.BoolToVisibilityMarkupConverter     UnvisibleValue$ Hidden=��� 5A      -   5        $��txtblkErrLogItemChanged��5A      $ 5,1,5,1q�5C      
���  ��$y�Hidden=�5E      ��5B      
  *resErrLogItemChanged+5E        5F      -   5           SmallImageSource$1 +/DBManager;component/RibbonIcons/Help_6.png��5F      5G      5H         ApplicationMenu 5K      =  7Microsoft.Windows.Controls.Ribbon.RibbonApplicationMenu  5L      .@�5M        -   5          Name$ ramiExportToXls��5M   (    	  ImageSource$3 -/DBManager;component/Icons/KEY_EXPORT_XLS.png��5O       / +RibbonApplicationMenuItemWithGoodImgScaling#��[ 5Q      ��5N      
  *resExportToXls+{�5P      �� *ExportToXlsEnabled+5R        5S      -   5        $ ramiExportFromXls��5S   7   $- '/DBManager;component/Icons/new_comp.png��5U      #��[ 5W      ��5T      
  *resCreateCompetition+{�5V      �� *CreateCompetitionEnabled+5X      5Y      .@�5\      1  +Microsoft.Windows.Controls.Ribbon.RibbonTab  -	   5          Name$ 
ribtabMain��5\      ��5]   
   
  *	resMain+.@�5^      3  -Microsoft.Windows.Controls.Ribbon.RibbonGroup  ��6   
  *resTableUpdating+.@�5_        -
   5        $��rbtnStartStop��5_        StartStopRibbonButtonStyle#��[ 5a       * &RibbonButtonWithGoodImgScalingTemplate#��[ 5b         Label 5`      
  *resAutoupdating+5c        5d      -   5           LargeImageSource$ /RibbonIcons/Refresh.png��5d      #��[ 5h       5e      
  *
resRefresh+{�5f      �� *RefreshEnabled+ � ToolTip 5g      
  *resRefreshToolTip+5i        5j      -   5        $&  /RibbonIcons/SyncDBWithFiles.png��5j      #��[ 5n       5k      
  *resSyncDBWithFiles+{�5l      �� *SyncDBWithFilesEnabled+ 5m      
  *resSyncDBWithFilesToolTip+5o        5p      -   5        $&  /RibbonIcons/SyncDBWithFiles.png��5p       5q      
  *resDBToGrid+{�5r      �� *DBToGridEnabled+ 5s      
  *resDBToGridToolTip+5t      5u        5w      ��6   
  *resTableContent+.@�5x      8  2Microsoft.Windows.Controls.Ribbon.RibbonMenuButton  -   5        $��
rmbtnGroup��5x         Label 5y      
  *resSelectGroup+{�5z      �� *CurrentGroups.Count+ 5  /DBManager.CollectionsCountToBoolMarkupConverter  ��5{      �� *CurrentGroups+ 5|        �� Path��%�   '	        �� 5}   	   " 	 6   .�5~   
   ��  � �� 5      �� $(  "Value.Value.AgeGroup.FullGroupName��� 5�   
    �@m�Command 5�      �� $  Value.Command�5�   
   5�   
   5�        5�      -   5        $��
rmbtnRound��5�       5�      
  *resSelectRound+{�5�      �� *CurrentRounds.Count+   ��5�      �� *CurrentRounds+ 5�      ��%�   '	        �� 5�   	   " 	 6   .�5�   
   ��  � �� 5�      �� $  Value.Value.Name��� 5�   
    �@m�Command 5�      �� $  Value.Command�5�   
   5�   	   5�        5�      -   5        $��rbtnOpenWorkbook��5�      $ /RibbonIcons/Open.png��5�       5�      
  *resOpenWorkbook+{�5�      �� *	IsEnabled+! �� ElementName$! 
rmbtnRound�� 5�      
  *resOpenWorkbookToolTip+5�      6  0Microsoft.Windows.Controls.Ribbon.RibbonCheckBox  5�      -   5        $��rchkShowGroupHead��5�      " d� 	IsChecked$
" TrueY�5�      #   Label# 5�      
  *resShowGroupHead+5�        5�      -   5        $��rchkAutoscrollEnabled��5�      $
" TrueY�5�      # 5�      
  *resAutoscrollEnabled+5�      5�        5�      ��6%   
  *resPublishing+{�6B   �� *PublishingEnabled+.@�5�        -   5        $) #/RibbonIcons/PublishingSettings.png��5�   *    5�      
  *resPublishingSettings+5�        5�      -   5        $) #/RibbonIcons/server-from-client.png��5�   *    5�      
  *resPublishResults+5�        5�      -   5        $!��rbtnPublishResultsLogWindow��5�   *   $# /RibbonIcons/CreateReport.png��5�       5�      
  *resPublishingLog+5�        5�      -   5        $��rchkAutoPublishing��5�   ,   # 5�      
  *resAutoPublishing+5�      5�      5�        5�      -   5        $ ribtabConfig��5�      ��5�   
   
  *	resConfig+.@�5�        $�� ��6   .@�5�        -   5        $ RibbonIcons/settings.png��5�      #��[ 5�       5�      
  *resSettings+{�5�      �� *SettingsEnabled+5�        5�      -   5        $��rbtnLogWindow��5�      $" RibbonIcons/CreateReport.png��5�       5�      
  *resLogWindow+{�5�      �� *LogWindowEnabled+5�        5�      -   5        $��rbtnShowRemoteControlWnd��5�      $* $RibbonIcons/if_teamviewer_100417.png��5�      #��[ 5�       5�      
  *resShowRemoteControlWnd+5�        5�      -   5        $��rbtnFalsestartRules��5�       5�      
  *resFalsestartRules+{�5�      �� *FalsestartRulesEnabled+5�        5�      # 6   
  *resGodsMode+" 5�      �� *
GodsMode+5�        5�      ��6   
  *	resGrades+{�62   �� *CalcGradesEnabled+.@�5�        -   5         5�      
  *resCalcGrades+5�        5�      -   5        $��mbtnHighlightGrades��5�      ��6:   �� *HighlightTypes+ 5�      ��%�   '	        �� 5�   	   " 	 6   .�5�   
   ��  � �� 5�      �� $  Value��� 5�   
    �@m�Command 5�      �� $  Command�5�   
   5�   	   5�      5�      5�      5�      � 5�      $ � RowDefinitions$ 5�      �� 5�      $
��auto �6   �� 5�      $
��auto �6   �� 5�      $
��auto �6   �� 5�      �� 5�      $
��auto �6   % � ColumnDefinitions% 5�      �� 5�      $��50��6   .+�5�      � -   5        $��grdGroupHead��5�      $��0��5�   
   $��0��5�       5�      ��%U   '��       �� 5�      " ��6   .�5�      ��  � ��$ Arial)�6%   5�      5�      $ 5�      �� 5      $
��auto �6   �� 5     $
��auto �6   �� 5     $
��auto �6   �� 5     $
��auto �6   �� 5     $
��auto �6   % 5     �� 5     �� 5	     .+�5     �� -   5        $��lblCompName��5  	   $��0��5     $��0��5  	   $��10��5  	   $��Center=�5  	   $��20(�5  	   $
��Bold"�5  	   �� 5     -    5        $��lblLocation��5  	   $��1��5     $��0��5  	   $
��Left=�5  	   $��14(�5  	   ��5  	   
  *resSmolensk+5  	   �� 5     -!   5        $��lblRoundDate��5  	   $��1��5     $��1��5  	   $��Right=�5  	   $��14(�5  	   �� 5     -"   5        $��lblRoundName��5!  	   $��2��5     $��0��5  	   $��10��5   	   $��Center=�5"  	   $��20(�5#  	   �� 5%     $ 
Horizontal=�6   $��3��5&     $��0��5'     $��10��5(      5)       	Row6Style& �� BasedOn' �� Triggers( �� Binding) �� Value��%�   &	       7���� 5*     " ��5+     8&   ' 5-  	   �� 5.  
   ( 6   �� *	Content+$! lblRow6��) 6G   Z� .a�5/     ��  � y�$ 	Collapsed=�6(   50     52  	   53     .+�55     �� $��0,0,0,0q�56  
   $��14(�57  
   ��55     
  *resMainJudge+57  
   �� 58     -#   5        $��lblMainJudge��58     $��0,0,10,0q�59  
   $��14(�5:  
   �� 5<     $��0,0,0,0q�5=  
   $��14(�5>  
   ��5<     
  *resMainSecretary+5>  
   �� 5?     -$   5        $��lblMainSecretary��5?     $��0,0,10,0q�5@  
   $��14(�5A  
   �� 5C     $��14(�5D  
   #��[ 5E  
   ��5C     
  *	resRow6+5E  
   �� 5F     -%   5        $��lblRow6��5F     $��0,0,0,0q�5G  
   $��14(�5H  
   #��[ 5I  
   5J     5K     �� 5W     -&   5        $%��scrlvwrAdditionalDataGridHeader��5X  	   $��2��5W     $��Arial)�5Y  	   $��14(�5Z  	   $��Disabled=�5[  	   $��Hidden=�5\  	   .��5]     �� -'   5        $ ��spAdditionalDataGridHeader��5]     $ 
Horizontal=�63   .+�5^     �� -(   5        $��lblLeftDummy��5^     $��21,0,0,0q�6!   $��0q�63   �� 5_     -)   5        $��lbRightDummy��5_     $��0,0,17,0q�6!   $��0q�63   5`     5a     � 5c     -*   5        $��grdRoundMembersHost��5c     $��3��6
   % 5d     �� 5e     $	��100��6   �� 5f     $
��auto �6   �� 5g     .+�5j     &   System.Windows.Controls.DataGrid  -+   5        $��dgrdRoundMembers��5k  	   $��0��5j     *   SelectionMode$* Single=�5m  	   +   AutoGenerateColumns+ .  ,   EnableRowVirtualization, . - $� VirtualizationMode$- Standard=�5p  	   . $� IsVirtualizing. . /   CanUserReorderColumns/ .  0   CanUserResizeRows0 .  1   CanUserSortColumns1 .  2   CanUserAddRows2 .  3   CanUserDeleteRows3 .  4   RowHeaderWidth$4 1Q�5w  	   5   
IsReadOnly5 . ��. $
��Auto=�5z  	   $
��Auto=�5{  	   6   MinColumnWidth$6 2Q�5|  	   $��14(�5}  	   $��Arial)�5~  	   7   ColumnHeaderHeight$7 30Q�5  	   ��5l  	   �� 8 �� Source#8 [  9   HorizontalGridLinesBrush9 5�  	   �� *$"GridLinesFontStyle.BackgroundColor+ B  <DBManager.Global.Converters.ColorToSolidBrushMarkupConverter  :   VerticalGridLinesBrush: 5�  	   �� *$"GridLinesFontStyle.BackgroundColor+   5�     �� 5�     -,   5        $��grdspltrRoundMembers��5�  
   $��1��5�     $��5��5�  
   ; �� ResizeDirection$; Columns=�5�  
   $��Stretch=�5�  
   $��Stretch=�5�  
     5�     --   5        $��dgrdRoundMembers2��5�  	   $��2��5�     $* Single=�5�  	   + .  , . $- Standard=�5�  	   . . / .  0 .  1 .  2 .  3 .  $4 1Q�5�  	   5 . ��. $
��Auto=�5�  	   $
��Auto=�5�  	   $6 2Q�5�  	   $��14(�5�  	   $��Arial)�5�  	   $7 30Q�5�  	   ��5�  	   �� #8 [ 9 5�     �� *$"GridLinesFontStyle.BackgroundColor+   : 5�  	   �� *$"GridLinesFontStyle.BackgroundColor+   5�     5�     .  (DBManager.RightPanels.CRightPanelControl  5�     -.   5        <  Name$< 
RightPanel��5�     $��4��5�     $��0��5�     $��Stretch=�5�     $��Stretch=�5�     5�     5�     5�     