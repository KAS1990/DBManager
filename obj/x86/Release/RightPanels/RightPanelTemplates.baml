   M S B A M L   `   `   `  ���� D  @DBManager, Version=1.0.0.1, Culture=neutral, PublicKeyToken=null=#clr-namespace:DBManager.RightPanelsDBManager.RightPanels  I)clr-namespace:DBManager.Global.ConvertersDBManager.Global.Converters  %clr-namespace:DBManager	DBManager  K*clr-namespace:DBManager.AttachedPropertiesDBManager.AttachedProperties  R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35` \RibbonControlsLibrary, Version=4.0.0.11019, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35J FWPFLocalization, Version=0.0.0.0, Culture=neutral, PublicKeyToken=nullR NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089  �� MergedDictionaries �� Source   StdLabelStyle �� 
TargetType �� Property �� Value  StdBoldLabelStyle �� BasedOn " FilteredMembersQTextBlockStyle �� StringFormat �� Triggers �� Binding	 �� Value  ColoredLabelStyle  ColoredLabelControl
 �� 
TargetType �� Padding �� Orientation
 � Tag �� Triggers P� 
SourceName P� Value P� Property �� 
TargetName:    4DBManager.AttachedProperties.RightPanelAttachedProps  InvitedToStartLabelStyle �� RelativeSource �� AncestorType"  WPFLocalization.LocExtension �� 	ConverterB   <DBManager.Global.Converters.ColorToSolidBrushMarkupConverter  PreparingLabelStyle   JustRecievedResultLabelStyle  StayOnStartLabelStyle 	 FalsestartLabelStyle  
 NextRoundMembersCountControl � DataContext;   5DBManager.Global.Converters.PeopleWordMarkupConverter  QualifRightPanel.   (DBManager.RightPanels.CRightPanelControl �� Padding � RowDefinitions � ColumnDefinitions%    ColoredLabelControlOrientation  MiddleRoundsRightPanel  FinalRightPanel  TotalRightPanel   DBManager.MainWindow5   /DBManager.RightPanels.GradesVisibilityConverter �� ConverterParameter2   ,DBManager.RightPanels.GradesContentConverter�� H 9http://schemas.microsoft.com/winfx/2006/xaml/presentation      :x,http://schemas.microsoft.com/winfx/2006/xaml     -local#clr-namespace:DBManager.RightPanels  4gconvs)clr-namespace:DBManager.Global.Converters  #mainwndclr-namespace:DBManager  <attachedprops*clr-namespace:DBManager.AttachedProperties    �� $$ ..\Global\GlobalResources.xaml?�%,   &	        &	 q     7   &	 �     &	     7   &	 �    7   &	 �    7  &	 {    7  &	 	    7  &	 �
    7  &		 �    7  &	
 x    7   7  7   7   7   &	 W    7   7  7   7   7  7  7   7 
 7  7  7  7  7 	 &	 ~    7   7  7   7   7  7   7 
 7  7 	 &	 �    7   7  7   7   7  7   7  7  7 	 &	 �    7   7  7  7   7  7  7  �� " ��.���  � ��$ 0,0,0,2q���  � ��$ Arial)���  � ��$ Center=���  � ��$ 16(��� " ��8   .���  � ��$
 Bold"���  � ��$ Right=���  � ��$ 24(��� " ��.���  � ��$ 0,0,0,2q���  �@��Padding$ 0,0.5,0.5,0.5q���  � ��$ Arial)���  � ��$ 24(���  � ��$
 Bold"���  � ��
 ��  ���  � �� �� *FilteredMembersQ+$ ({0})�� ��  �� *FilteredMembersQ+	 Z� .a���  � y�$ 	Collapsed=��� " ��8   .���  � ��$ 2,2,2,2q���  � ��$ Center=���  � ��$ Center=���  � ��$ Stretch=���  � ��
 �   ���  � ��$ 1q���  � ��$ 2,0.5,2,0.5q��� " ��.���  � �� �� "
 ��.R��� $��PART_Border��$��2,2,2,2q�$ 2,0.5,2,0.5q�
���   �$��1q�$��Center=�#��z��._��� $��
PART_Panel��$ Vertical=�$��Center=�.+��� $��
PART_Title��$��	0,1.5,0,2q�$��0q�#��z��$��Center=�#��z��#��z��#��z��8��  �� $��PART_Member��$��	0,0,0,1.5q�$��0,0,0,0q�#��z $��Center=�#��z��$��18(�$
��Bold"� P� $ PART_Member�� Z�  � ��. ��� $ PART_Member�� � y�$ 	Collapsed=�P� & �@  ColoredLabelControlOrientation$ 
Horizontal=�. ��� $ PART_Border�� � ��$
 auto���� $ PART_Border�� � ��$ Stretch=��� $ 
PART_Panel�� �@��Orientation$ 
Horizontal=��� $ 
PART_Title�� � ��$ 0,0,4,0q��� $ PART_Member�� � ��$ 0,0,0,0q��� " ��8   .���  �@�DataContext �� *InvitedToStartFontStyle+ �� $ local:CRightPanelControlL���  � ��   *resInvitedToStart+��  �@�Tag �� *InvitedToStartMember+ �� $ local:CRightPanelControlL���  � �� �� *BackgroundColor+   ��  � �� �� *	ForeColor+   ��  � �� �� *	FontStyle+��  � �� �� *
FontWeight+�� " ��8   .���  �@�DataContext �� *PreparingFontStyle+ �� $ local:CRightPanelControlL���  � ��   *resPreparing+��  �@�Tag �� *PreparingMember+ �� $ local:CRightPanelControlL���  � �� �� *BackgroundColor+   ��  � �� �� *	ForeColor+   ��  � �� �� *	FontStyle+��  � �� �� *
FontWeight+�� " ��8   .���  �@�DataContext �� *JustRecievedResultFontStyle+ �� $ local:CRightPanelControlL���  � ��   *resJustRecievedResult+��  � �� �� *BackgroundColor+   ��  � �� �� *	ForeColor+   ��  � �� �� *	FontStyle+��  � �� �� *
FontWeight+��  � ��$ 5,0.5,5,0.5q��� " ��8   .���  �@�DataContext �� *StayOnStartFontStyle+ �� $ local:CRightPanelControlL���  � ��   *resStayOnStart+��  � �� �� *BackgroundColor+   ��  � �� �� *	ForeColor+   ��  � �� �� *	FontStyle+��  � �� �� *
FontWeight+��  � ��$ 5,0.5,5,0.5q��� " ��8   .���  �@�DataContext �� *FalsestartFontStyle+ �� $ local:CRightPanelControlL���  � ��   *resFalsestartInOneLine+��  � �� �� *BackgroundColor+   ��  � �� �� *	ForeColor+   ��  � �� �� *	FontStyle+��  � �� �� *
FontWeight+��  � ��$ 5,0.5,5,0.5q���  � y�$ 	Collapsed=��� " ��.���  � �� �� .R��� $ 
Horizontal=�.+��� $��2,2,7,2q�
���   �$��1q�$
��Left=� �� * NextRoundMembersCountFontStyle+ �� $ local:CRightPanelControlL����� *BackgroundColor+   ._��� $ 
Horizontal=�.+��� $��0,0,2,0q�$��2,2,2,2q�8��  ��  *resNextRoundMembersCount+���� *	ForeColor+   ���� *	FontStyle+���� *
FontWeight+�� $��0,0,0,0q�$��2,2,2,2q�8�� ���� *NextRoundMembersQ+ �� $ local:CRightPanelControlL����� *	ForeColor+   ���� *	FontStyle+�� $��0,0,0,0q�$��2,2,2,2q�8�� ���� *NextRoundMembersQ+ �� $ local:CRightPanelControlL�   ���� *	ForeColor+   ���� *	FontStyle+���� *
FontWeight+�� $��2,2,2,2q�
���   �$��1q�._��� $ 
Horizontal=�.+��� 8�� $��0,0,0,0q�$��2,2,2,2q���  *resResultsOnSite+�� 8�� $��0,0,0,0q�$��2,2,2,2q�
����  ���  *resSmolclimbSite+�� "
  .R��� 
���   �$��1q�$ 2q�._��� $ Vertical=�.+��� $ 
Horizontal=�$��2,0,2,0q�.+��� 8��  $��0,0,5,0q���  *resWholeMembersCount+�� 8�� $��0,0,5,0q����� *WholeMembersQ+�� 8�� $��0,0,0,0q����� *WholeMembersQ+   �� $��20,0,0,0q�8�� $��0,0,5,0q���  *resRoundMembersCount+�� $ 
Horizontal=�$��Right=�$��Center=�.+��� 8�� $��0,0,5,0q����� *RoundMembersQ+�� 8�� $ 0,0,5,0q��� 8�� $��0,0,0,0q����� *RoundMembersQ+   �� 8�� �  �� $
��auto ��� $
��auto � �� $
��auto ��� $
��auto ��� $
��auto �.+��� $��0��$��0��$ 
Horizontal=�8�� �� $��1��$��0��$ 
Horizontal=�8��	 �� $��0��$��1��8��
 �� $��1��$��1��8�� �� $��1��$��2��8�� �� "
  .R��� 
���   �$��1q�$ 2q�._��� $ Vertical=�.+��� $ 
Horizontal=�.+��� 8��  $��2,0.5,5,0.5q���  *resWholeMembersCount+�� 8�� $��0,0.5,5,0.5q����� *WholeMembersQ+�� 8�� $��0,0.5,0,0.5q����� *WholeMembersQ+   �� $��20,0,0,2q�8�� $��2,0.5,5,0.5q���  *resRoundMembersCount+�� 8�� $��0,0.5,5,0.5q����� *RoundMembersQ+�� 8�� $��0,0.5,0,0.5q����� *RoundMembersQ+   �� 8�� �� $ 
Horizontal=�.+��� 8�� �� 8�� �� "
  .R��� 
���   �$��1q�$ 10q�._��� $ Vertical=�.+��� $ 
Horizontal=�.+��� 8��  $��2,0.5,5,0.5q���  *resWholeMembersCount+�� 8�� $��0,0.5,5,0.5q����� *WholeMembersQ+�� 8�� $��0,0.5,0.5,0.5q����� *WholeMembersQ+   �� $��20,0,0,2q�8�� $��2,0.5,5,0.5q���  *resRoundMembersCount+�� 8�� $��0,0.5,5,0.5q����� *RoundMembersQ+�� 8�� $��0,0.5,0,0.5q����� *RoundMembersQ+   �� $
��Left=�8�� ��  *
resWinners+ �� * NextRoundMembersCountFontStyle+ �� $ local:CRightPanelControlL����� *BackgroundColor+   ���� *	ForeColor+   ���� *	FontStyle+���� *
FontWeight+�� $ 
Horizontal=�.+��� 8�� �� 8�� �� "
  .R��� 
���   �$��1q�$ 10q�._��� $ Vertical=�.+��� $ 
Horizontal=�.+��� 8��  ��  *resWholeMembersCount+�� $ 
Horizontal=�$��Right=�$��Center=�.+��� 8�� ���� *WholeMembersQ+�� 8�� �� 8�� ���� *WholeMembersQ+   �� $��20���� ���� " ��8  .���  �@�DataContext �� *InvitedToStartFontStyle+ �� $ local:CRightPanelControlL���  � ��   *resAdultGrades+��  � ��$	 250����  � �� �� *BackgroundColor+   ��  � y� �� *$"DataContext.CurHighlightGradesType+ �� "     $ 0���� ���� " ��8  .���  �@�DataContext �� *StayOnStartFontStyle+ �� $ local:CRightPanelControlL���  � ��   *resYoungGrades+��  � ��$	 250����  � �� �� *BackgroundColor+   ��  � y� �� *$"DataContext.CurHighlightGradesType+ �� "     $ 0���� ���� " ��8  .���  �@�DataContext �� *PreparingFontStyle+ �� $ local:CRightPanelControlL���  � �� �� *$"DataContext.CurHighlightGradesType+ �� "     ��  � ��$	 500����  � �� �� *BackgroundColor+   ��  � y� �� *$"DataContext.CurHighlightGradesType+ �� "     $ 1��