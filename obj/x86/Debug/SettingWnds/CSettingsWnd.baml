   M S B A M L   `   `   `  ����D  @DBManager, Version=1.0.0.1, Culture=neutral, PublicKeyToken=null=#clr-namespace:DBManager.SettingWndsDBManager.SettingWnds  5      I)clr-namespace:DBManager.InterfaceElementsDBManager.InterfaceElements  5      (    "DBManager.SettingWnds.CSettingsWnd   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35` \RibbonControlsLibrary, Version=4.0.0.11019, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35J FWPFLocalization, Version=0.0.0.0, Culture=neutral, PublicKeyToken=nullH 9http://schemas.microsoft.com/winfx/2006/xaml/presentation      5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      7baseWnd#clr-namespace:DBManager.SettingWnds     5      @
localctrls)clr-namespace:DBManager.InterfaceElements     5        � ShowInTaskbar  .   � WindowStyle$ 
ToolWindow=�5       � WindowStartupLocation$ CenterScreen=�5	       � SizeToContent$ WidthAndHeight=�5
       � 
ResizeMode$ NoResize=�5      /   )DBManager.SettingWnds.СCustomSettingsWnd   ResMsgOnCloseIfModified$ resSettingChanged��5       � Title 5      "  WPFLocalization.LocExtension  *resSettingsWndTitle+.��5      ]� ��. .+�5      ]� $��Bottom=�6   .+�5      .   (DBManager.InterfaceElements.CancelButton  -   5          Name$ 	btnCancel��5      $��Right=�5      $��5q�5      $��Right=�5      ��5        *	resCancel+5      �� 5      -   5        $��btnApply��5      $��Right=�5      $��5,5,5,5q�5      $��Right=�5         cmdApply#��Z 5      ��5        *
resApply+5      �� 5      -   5        $��btnOK��5      $��Right=�5      $��5q�5       $��Right=�5!      ��5        *resOK+5"      �� 5$      -   5        $��btnToDefault��5%      $��Right=�5$      $��5q�5'      $
��Left=�5(      ��5&        *resToDefault+5)      5*      � 5,      $��	10,0,10,0q�6	   	 � RowDefinitions	 5-      �� 5.      $
��auto �6   �� 5/      $
��auto �6   �� 50      $
��auto �6   �� 51      $
��auto �6   �� 52      $
��auto �6   �� 53      $
��auto �6   �� 54      $
��auto �6   �� 55      $
��auto �6   �� 56      $
��auto �6   �� 57      $
��auto �6   �� 58      $
��auto �6   �� 59      $
��auto �6   �� 5:      $
��auto �6   �� 5;      $
��auto �6   �� 5<      $
��auto �6   �� 5=      $
��auto �6   �� 5>      $
��auto �6   �� 5?      $
��auto �6    �� 5@      $
��auto �6    �� 5A      $
��auto �6    
 � ColumnDefinitions
 5D      �� 5E      $
��auto �6   �� 5F      $
��auto �6   �� 5G      $
��auto �6   �� 5H      $��* �6   .+�5K      �� -   5        $��chkAutodetectOnStart��5N      $��0��5K      $��0��5L      $��10��5M      $��Center=�5O      $��5,0,0,5q�5Q      ��5P        *resAutodetectOnStart+5S      �� 5V      $��1��6   $��0��5W      $��0,0,0,5q�5Y      $��Center=�5Z      ��5X        *	resComp+5Z      �� 5[      -   5        $��cmbComp��5]      $��1��5[      $��1��5\      $��0,0,5,5q�5^      $��60��5_      $	��550��5`       �� 
IsEditable . 5b      �� 5c      -   5        $��1��5c      $��2��5d      $	��...��5e      $��75��5f      $��0,0,0,5q�5g      $��5q�5h      $��Center=�5i      5j      �� 5l      -   5        $��chkHandleFileDeletion��5o      $��2��5l      $��0��5m      $��10��5n      $��Center=�5p      $��5,0,0,5q�5r      ��5q        *resHandleFileDeletion+5t      �� 5v      $��3��6   $��0��5w   	   $��0,0,0,5q�5y   	    �� Padding$ 5q�5z   	   $	��200��5{   	   $
��Wrap=�5|   	   ��5x   	     *resResultGradeCalcMethod+5|   	   �� 5}      -	   5        $��cmbResultGradeCalcMethod��5      $��3��5}      $��1��5~      $��0,0,5,5q�5�      $��Center=�5�      $
��Left=�5�      .@�5�      �� ��6     *resResultGradeCalcMethodFloor+�� 5�      ��6     *resResultGradeCalcMethodRound+5�      �� 5�      -
   5        $#��chkOnly75PercentForCalcGrades��5�      $��4��5�      $��0��5�      $��10��5�      $��Center=�5�      $��5,0,0,5q�5�      ��5�        *resOnly75PercentForCalcGrades+5�      �� 5�      $��5��6   $��0��6   $��0,0,0,5q�6R   ��6(     *resResultGradesSetFromAge+6R   �� 5�      $��5��6   $��1��6    �� Orientation$ 
Horizontal=�6-   .+�5�      +   %DBManager.InterfaceElements.TextBoxEx  -   5          Name$  txtMinAgeToCalcResultGrade��5�      +   %DBManager.InterfaceElements.TextBoxEx   TextChanged$, &txtMinAgeToCalcResultGrade_TextChanged��5�      $��50��5�       �� 	MaxLength$ 2��5�      $��0,0,0,5q�5�      $
��Left=�5�         InputCheckType$ PositiveInt=�5�         	FieldName 5�        *resResultGradesSetFromAge+5�      �� 5�      -   5        $!��lblMaxYearToCalcResultGrade��5�      5�      �� 5�      $��6��6   $��0��6!   $��0,0,0,5q�6[   ��61     *resWorkbookTemplateFolder+6[     5�      -   5        $ txtWorkbookTemplateFolder��5�   #   $ txt_TextChanged��5�   #   $��6��5�   
   $��1��6   $	��520��5�   $   $��0,0,0,5q�5�   
   $ NotEmpty=�5�   
    5�   
     *resWorkbookTemplateFolder+5�   #   �� 5�      -   5        $��6��5�      $��2��5�      $	��...��5�      $��75��5�      $��0,0,0,5q�5�      $��5q�5�      5�      �� 5�      $��7��6   $��0��6!   $��0,0,0,5q�6M   ��61     *resFontSize+6M     5�      -   5        $ txtFontSize��5�      $ txt_TextChanged��5�         	LostFocus$ txtFontSize_LostFocus��5�         
MouseWheel$ txtFontSize_MouseWheel��5�      $��7��5�      $��1��5�      $��50��5�      $��0,0,0,5q�5�      $
��Left=�5�      $ PositiveInt=�5�       5�        *resFontSize+5�      �� 5�      -   5        $��8��5�      $��0��5�      $��0,0,0,5q�5�      $��23��5�      $��Bottom=�5�      ��5�        *resFontFamily+5�      �� 5�      -   5        $��lblFontFamilyName��5�      $��8��5�      $��1��5�      $��0,0,0,5q�5�      5�      2   ,DBManager.InterfaceElements.CFontStylePicker  5�      -   5          Name$ fntstlGridLines��5�      $��9��5�   *   $��0��5�      $��10��5�         FieldsToShow$ BackgroundColor=�5�   *      
TextSample 5�        *resGridLines+5�   *     5�      -   5        $ fntstlPlainResults��5�      $��10��5�   *   $��0��5�      $��10��5�      $ ForeColor,BackgroundColor=�5�       5�        *resPlainResults+5�        5�      -   5        $! fntstlNextRoundMembersCount��5�      $��11��5�   !   $��0��5�      $��10��5�       5�        *#!resNextRoundMembersCountInOneLine+  5�      -   5        $ fntstlInvatedToStart��5�      $��12��5�   !   $��0��5�      $��10��5�       5�        *resInvitedToStartInOneLine+  5�      -   5        $ fntstlPreparing��5�      $��13��5�   !   $��0��5�      $��10��5�       5�        *resPreparingInOneLine+  5�      -   5        $ fntstlStayOnStart��5�      $��14��5�   !   $��0��5�      $��10��5�       5�        *resStayOnStartInOneLine+  5�      -   5        $ fntstlJustRecievedResult��5�      $��15��5�   !   $��0��5�      $��10��5�       5�        * resJustRecievedResultInOneLine+  5�      -   5        $ fntstlFalsestart��5�      $��16��5�   !   $��0��5�      $��10��5�       5�        *resFalsestartInOneLine+5�      5�      5�      