   M S B A M L   `   `   `  ���� D  @DBManager, Version=1.0.0.1, Culture=neutral, PublicKeyToken=null3clr-namespace:DBManager.GlobalDBManager.Global      DBManager.Stuff.CLogWnd   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35` \RibbonControlsLibrary, Version=4.0.0.11019, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35J FWPFLocalization, Version=0.0.0.0, Culture=neutral, PublicKeyToken=nullH 9http://schemas.microsoft.com/winfx/2006/xaml/presentation      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     /globclr-namespace:DBManager.Global     0   *DBManager.Global.CNotifyPropertyChangedWnd    SizeChanged$  CLogWnd_SizeChanged��   Loaded$ CLogWnd_Loaded�� � ShowInTaskbar .   � WindowStyle$ 
ToolWindow=� � WindowStartupLocation$ CenterScreen=� � SizeToContent$ WidthAndHeight=� � Title "  WPFLocalization.LocExtension  *resLogWndTitle+.��]� ��. $��5,5,5,5q�.+��� $	��Top=���  *resLogFilePath+�� -   $��txtLogFilePath��$	��Top=�$��0,0,0,15q�$��5q� �� 
IsReadOnly . $
��Left=����� *ActualWidth+ �� ElementName$ 	lstvItems���� $	��Top=�	 �� Orientation$	 
Horizontal=�$��0,0,0,5q�.+��� -   $��0,0,5,0q�$��5q���  *
resRefresh+�� -   $��0,0,5,0q�$��5q���  *
resClear+� -   $��grdItems��.+��� -   $��	lstvItems��$	��400��$	��500��$	��400��$	��400����.  ���� *Items+
 � 	Resources
  �� 
TargetType �� Property �� Value��%[   '��       �� " ��.���  � ��$ Stretch=���  � ��$ 1q���  � ��# Z	� �� View ��  �� AllowsColumnReorder .  .P���  �� DisplayMemberBinding �� *CreationDateInString+���� -    ��Name$ grdcolhdrCreationDate��.���� $��Stretch=�$��20�� �� Padding$ 2,5,2,0q���  *resCreationDate+�� ���� -   $��txtblkExceptionsHeader��$��20��$ 5,5,2,0q�$
��Left=���  *resExceptions+ �� CellTemplate �� .R��� $��Stretch=�$	��300����.  ���� *
Exceptions+���� .R�]� ��. .+��� $	��Top=�$ 2q�$
��Bold"����� *IndexInString+�� 
�������$��1q�._��� $	��200��.����  . 
������ ���� *Text+