   M S B A M L   `   `   `  ���� D  @DBManager, Version=1.0.0.1, Culture=neutral, PublicKeyToken=null=#clr-namespace:DBManager.SettingWndsDBManager.SettingWnds  I)clr-namespace:DBManager.InterfaceElementsDBManager.InterfaceElements  (    "DBManager.SettingWnds.CSettingsWnd   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35` \RibbonControlsLibrary, Version=4.0.0.11019, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35J FWPFLocalization, Version=0.0.0.0, Culture=neutral, PublicKeyToken=nullH 9http://schemas.microsoft.com/winfx/2006/xaml/presentation      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     7baseWnd#clr-namespace:DBManager.SettingWnds     @
localctrls)clr-namespace:DBManager.InterfaceElements       � ShowInTaskbar  .   � WindowStyle$ 
ToolWindow=� � WindowStartupLocation$ CenterScreen=� � SizeToContent$ WidthAndHeight=� � 
ResizeMode$ NoResize=�/   )DBManager.SettingWnds.СCustomSettingsWnd   ResMsgOnCloseIfModified$ resSettingChanged�� � Title "  WPFLocalization.LocExtension  *resSettingsWndTitle+.��]� ��. .+�]� $��Bottom=�.+�.   (DBManager.InterfaceElements.CancelButton  -     Name$ 	btnCancel��$��Right=�$��5q�$��Right=���  *	resCancel+�� -   $��btnApply��$��Right=�$��5,5,5,5q�$��Right=�   cmdApply#��Z ��  *
resApply+�� -   $��btnOK��$��Right=�$��5q�$��Right=���  *resOK+�� -   $��btnToDefault��$��Right=�$��5q�$
��Left=���  *resToDefault+� $��	10,0,10,0q�	 � RowDefinitions	 �� $
��auto ��� $
��auto ��� $
��auto ��� $
��auto ��� $
��auto ��� $
��auto ��� $
��auto ��� $
��auto ��� $
��auto ��� $
��auto ��� $
��auto ��� $
��auto ��� $
��auto ��� $
��auto ��� $
��auto ��� $
��auto ��� $
��auto ��� $
��auto ��� $
��auto ��� $
��auto �
 � ColumnDefinitions
 �� $
��auto ��� $
��auto ��� $
��auto ��� $��* �.+��� -   $��chkAutodetectOnStart��$��0��$��0��$��10��$��Center=�$��5,0,0,5q���  *resAutodetectOnStart+�� $��1��$��0��$��0,0,0,5q�$��Center=���  *	resComp+�� -   $��cmbComp��$��1��$��1��$��0,0,5,5q�$��60��$	��550�� �� 
IsEditable . �� -   $��1��$��2��$	��...��$��75��$��0,0,0,5q�$��5q�$��Center=��� -   $��chkHandleFileDeletion��$��2��$��0��$��10��$��Center=�$��5,0,0,5q���  *resHandleFileDeletion+�� $��3��$��0��$��0,0,0,5q� �� Padding$ 5q�$	��200��$
��Wrap=���  *resResultGradeCalcMethod+�� -	   $��cmbResultGradeCalcMethod��$��3��$��1��$��0,0,5,5q�$��Center=�$
��Left=�.@��� ��  *resResultGradeCalcMethodFloor+�� ��  *resResultGradeCalcMethodRound+�� -
   $#��chkOnly75PercentForCalcGrades��$��4��$��0��$��10��$��Center=�$��5,0,0,5q���  *resOnly75PercentForCalcGrades+�� $��5��$��0��$��0,0,0,5q���  *resResultGradesSetFromAge+�� $��5��$��1�� �� Orientation$ 
Horizontal=�.+�+   %DBManager.InterfaceElements.TextBoxEx  -     Name$  txtMinAgeToCalcResultGrade��+   %DBManager.InterfaceElements.TextBoxEx   TextChanged$, &txtMinAgeToCalcResultGrade_TextChanged��$��50�� �� 	MaxLength$ 2��$��0,0,0,5q�$
��Left=�   InputCheckType$ PositiveInt=�   	FieldName   *resResultGradesSetFromAge+�� -   $!��lblMaxYearToCalcResultGrade���� $��6��$��0��$��0,0,0,5q���  *resWorkbookTemplateFolder+  -   $ txtWorkbookTemplateFolder��$ txt_TextChanged��$��6��$��1��$	��520��$��0,0,0,5q�$ NotEmpty=�   *resWorkbookTemplateFolder+�� -   $��6��$��2��$	��...��$��75��$��0,0,0,5q�$��5q��� $��7��$��0��$��0,0,0,5q���  *resFontSize+  -   $ txtFontSize��$ txt_TextChanged��   	LostFocus$ txtFontSize_LostFocus��   
MouseWheel$ txtFontSize_MouseWheel��$��7��$��1��$��50��$��0,0,0,5q�$
��Left=�$ PositiveInt=�   *resFontSize+�� -   $��8��$��0��$��0,0,0,5q�$��23��$��Bottom=���  *resFontFamily+�� -   $��lblFontFamilyName��$��8��$��1��$��0,0,0,5q�2   ,DBManager.InterfaceElements.CFontStylePicker  -     Name$ fntstlGridLines��$��9��$��0��$��10��   FieldsToShow$ BackgroundColor=�   
TextSample   *resGridLines+  -   $ fntstlPlainResults��$��10��$��0��$��10��$ ForeColor,BackgroundColor=�   *resPlainResults+  -   $! fntstlNextRoundMembersCount��$��11��$��0��$��10��   *#!resNextRoundMembersCountInOneLine+  -   $ fntstlInvatedToStart��$��12��$��0��$��10��   *resInvitedToStartInOneLine+  -   $ fntstlPreparing��$��13��$��0��$��10��   *resPreparingInOneLine+  -   $ fntstlStayOnStart��$��14��$��0��$��10��   *resStayOnStartInOneLine+  -   $ fntstlJustRecievedResult��$��15��$��0��$��10��   * resJustRecievedResultInOneLine+  -   $ fntstlFalsestart��$��16��$��0��$��10��   *resFalsestartInOneLine+