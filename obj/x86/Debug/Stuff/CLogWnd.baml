   M S B A M L   `   `   `  ����D  @DBManager, Version=1.0.0.1, Culture=neutral, PublicKeyToken=null3clr-namespace:DBManager.GlobalDBManager.Global  5   	       DBManager.Stuff.CLogWnd   5   !   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35` \RibbonControlsLibrary, Version=4.0.0.11019, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35J FWPFLocalization, Version=0.0.0.0, Culture=neutral, PublicKeyToken=nullH 9http://schemas.microsoft.com/winfx/2006/xaml/presentation      5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   /globclr-namespace:DBManager.Global     5   	   0   *DBManager.Global.CNotifyPropertyChangedWnd    SizeChanged$  CLogWnd_SizeChanged��5
   	      Loaded$ CLogWnd_Loaded��5   	    � ShowInTaskbar .   � WindowStyle$ 
ToolWindow=�5   	    � WindowStartupLocation$ CenterScreen=�5   	    � SizeToContent$ WidthAndHeight=�5	   	    � Title 5   	   "  WPFLocalization.LocExtension  *resLogWndTitle+.��5      ]� ��. $��5,5,5,5q�6"   .+�5   
   �� $	��Top=�6   ��6%     *resLogFilePath+�� 5      -   5        $��txtLogFilePath��5      $	��Top=�5      $��0,0,0,15q�5      $��5q�5       �� 
IsReadOnly . $
��Left=�5      ��5      �� *ActualWidth+ �� ElementName$ 	lstvItems���� 5      $	��Top=�6   	 �� Orientation$	 
Horizontal=�6$   $��0,0,0,5q�6=   .+�5      �� -   5        $��0,0,5,0q�5   @   $��5q�6Q   ��6     *
resRefresh+6Q   �� 5      -   5        $��0,0,5,0q�5   <   $��5q�6M   ��6     *
resClear+6M   5      � 5      -   5        $��grdItems��5   	   .+�5      �� -   5        $��	lstvItems��5      $	��400��5      $	��500��5      $	��400��5       $	��400��5!      ��.  ��5#      �� *Items+
 � 	Resources
 5&       �� 
TargetType �� Property �� Value��%�   '��       �� 5'      " ��6   .�5(      ��  � ��$ Stretch=�65   �� 5)       � ��$ 1q�6*   �� 5*       � ��# Z	�6&   5+      5,       �� View 5.      �� 5/       �� AllowsColumnReorder .  .P�50      ��  �� DisplayMemberBinding 6   �� *CreationDateInString+��51   	   �� 52   
   -   5         ��Name$ grdcolhdrCreationDate��52      .��53      �� $��Stretch=�6   $��20��63    �� Padding$ 2,5,2,0q�6?   ��6Q     *resCreationDate+54      56   	   �� 58      ��59   	   �� 5:   
   -   5        $��txtblkExceptionsHeader��5:      $��20��5<      $ 5,5,2,0q�6   $
��Left=�5=      ��5;        *resExceptions+5=       �� CellTemplate 5@   	   �� 5A   
   .R�5B      �� $��Stretch=�5C      $	��300��5D      ��.  ��5B      �� *
Exceptions+��5F      �� 5G      .R�5H      ]� ��. .+�5I      �� $	��Top=�6   $ 2q�5K      $
��Bold"�5L      ��5J      �� *IndexInString+5L      �� 5M      
�������$��1q�6)   ._�5N      �� $	��200��6   .��5O      ��  . 
������ ��5P      �� *Text+5Q      5R      5S      5T      5U      5W      5X      5Z   	   5[      5]      5^      5_      5`      