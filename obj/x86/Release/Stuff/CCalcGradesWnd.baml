   M S B A M L   `   `   `  ���� D  @DBManager, Version=1.0.0.1, Culture=neutral, PublicKeyToken=null3clr-namespace:DBManager.GlobalDBManager.Global  $    DBManager.Stuff.CCalcGradesWnd   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35` \RibbonControlsLibrary, Version=4.0.0.11019, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35J FWPFLocalization, Version=0.0.0.0, Culture=neutral, PublicKeyToken=nullH 9http://schemas.microsoft.com/winfx/2006/xaml/presentation      1globclr-namespace:DBManager.Global      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml       � ShowInTaskbar  .   � WindowStyle$ 
ToolWindow=� � WindowStartupLocation$ CenterScreen=� � SizeToContent$ WidthAndHeight=� � Title "  WPFLocalization.LocExtension  *resCalcGradesWndTitle+.��]� ��. $��5,0,5,0q�.+��� -   $��chkSelectAll��$	��Top=�$��0,5,0,5q���  *resSelectAll+�� $��Bottom=�$��Right=� �� Orientation$ 
Horizontal=�.+��� -   $��5q�$��5q���  *resRemoveGradesBtnContent+�� -   $��5q�$��5q���  *resCalcPlacesBtnContent+{��� *CalcEnabled+�� -   $��5q�$��5q���  *resCalcGradesBtnContent+{��� *CalcEnabled+�� $��5q�$��5q���  *
resClose+�� $��Bottom=�$��0,10,0,5q���  *resCalcGradesResults+.��� -   $��grdCalcGradesResults��$��2,5,2,2q� � 	Resources  �� 
TargetType �� Property	 �� Value��%1   '��       �� " ��.���  � ��$	 Center=�
 � RowDefinitions
 �� �� �� �� �� �� ��  � ColumnDefinitions �� $	��100 ��� $	��100 �.+��� $��0��$��0��$��0,0,0,1q�
���������  *
resGrade+�� $��1��$��0��$��0,0,0,1q�
���������  *
resPlace+�� $��Bottom=�$ Vertical=�.+��� $��0,0,0,0q�$
��Left=���  *resResultGradeCalcMethod+�� -   $��cmbResultGradeCalcMethod��$��0,0,0,5q�$
��Left=�.@��� ��  *resResultGradeCalcMethodFloor+�� ��  *resResultGradeCalcMethodRound+�� -   $#��chkOnly75PercentForCalcGrades��$��Center=�$��0,0,0,5q���  *resOnly75PercentForCalcGrades+�� $	��170��$��0,0,0,5q�$��Stretch=���. $
��Auto=�$
��Auto=����� *	CompYears+���� .R��� $��2q�$��Center=� d� 	IsChecked �� *
IsSelected+���� *Name+���� " ��.���  �@�FocusVisualStyle	 Z� ��  � |�	 .  