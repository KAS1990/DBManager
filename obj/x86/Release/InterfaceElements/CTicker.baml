   M S B A M L   `   `   `  ���� D  @DBManager, Version=1.0.0.1, Culture=neutral, PublicKeyToken=null3clr-namespace:DBManager.GlobalDBManager.Global  I)clr-namespace:DBManager.Global.ConvertersDBManager.Global.Converters  I)clr-namespace:DBManager.InterfaceElementsDBManager.InterfaceElements  )    #DBManager.InterfaceElements.CTicker   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35` \RibbonControlsLibrary, Version=4.0.0.11019, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35J FWPFLocalization, Version=0.0.0.0, Culture=neutral, PublicKeyToken=nullH 9http://schemas.microsoft.com/winfx/2006/xaml/presentation      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     /globclr-namespace:DBManager.Global     ;convs)clr-namespace:DBManager.Global.Converters     ;local)clr-namespace:DBManager.InterfaceElements     $��0q�  � 	Resources  9   3DBManager.InterfaceElements.DurationTickerConverter   valconvDurationTicker@   :DBManager.InterfaceElements.DurationTickerOneTimeConverter   valconvDurationTickerOneTime?   9DBManager.InterfaceElements.FirstBeginTimeTickerConverter  valconvFirstBeginTimeTicker �� 
TargetType �� Property �� Value �� RelativeSource �� AncestorType  Ticker1OneTimeStoryboard g� RepeatBehavior g� FillBehavior �� 
TargetName	 �� TargetProperty
 W� From	 W� To `� 	ConverterD   >DBManager.InterfaceElements.LastLeftMultiTickerMarkupConverter �� Path �� ElementName g� Duration O� KeyTime O� Value  Ticker1Storyboard `� ConverterParameter g� 	BeginTime �� 	Converter?   9DBManager.InterfaceElements.LastLeftTickerMarkupConverter  Ticker2HideStoryboard  Ticker2Storyboard��%�  &	        &	      &	 
     '��      &	 �    7  7  &	     7   7  7   &	 �    7  &	 �    7   7  7         �� " ��.���  � ��$ 0q���  �@��Padding$ 0q���  � ��$ Center=���  � ��$ Center=���  � ��
 ���� ��  �@��Left$ 0����  � �� �� *
TickerText+ �� *FindAncestor+$ local:CTickerL���  � �� �� *
FontSize+ �� *FindAncestor+$ local:CTickerL���  � �� �� *
FontFamily+ �� *FindAncestor+$ local:CTickerL���  � �� �� *	FontStyle+ �� *FindAncestor+$ local:CTickerL���  � �� �� *
FontWeight+ �� *FindAncestor+$ local:CTickerL���  � �� �� *
Foreground+ �� *FindAncestor+$ local:CTickerL��� $ 1x��$ HoldEnd=�.��W� $ txtblkTickerOneTime��$	 (Canvas.Left)�$
 0X� `�    .1��� $ ActualWidth�$ 
cnvsTicker���� $ ActualWidth�$ txtblkTicker1�� `� 8   .1��� $ ActualWidth�$ 
cnvsTicker���� $ ActualWidth�$ txtblkTicker1���� $ TickerSpeed� �� *FindAncestor+$ local:CTickerL�U� $ txtblkTicker1��$	 Opacity�$
 Stop=� `� 8  .1��� $ ActualWidth�$ 
cnvsTicker���� $ ActualWidth�$ txtblkTicker1���� $ TickerSpeed� �� *FindAncestor+$ local:CTickerL�.\�n� $ 0��$ 0Q��� $ Forever�� `� 8   $ 0��.1��� $ ActualWidth�$ 
cnvsTicker���� $ ActualWidth�$ txtblkTicker1���� $ TickerSpeed� �� *FindAncestor+$ local:CTickerL� `� 8  .1��� $ ActualWidth�$ 
cnvsTicker���� $ ActualWidth�$ txtblkTicker1���� $ TickerSpeed� �� *FindAncestor+$ local:CTickerL�.��W� $ txtblkTicker1��$	 (Canvas.Left)�
 �� *ActualWidth+$ 
cnvsTicker�� �� *ActualWidth+$ txtblkTicker1��    `� 8  $ 1��.1��� $ ActualWidth�$ 
cnvsTicker���� $ ActualWidth�$ txtblkTicker1���� $ TickerSpeed� �� *FindAncestor+$ local:CTickerL��� $ 1x��$
 Stop=� `� 8   .1��� $ ActualWidth�$ 
cnvsTicker���� $ ActualWidth�$ txtblkTicker1���� $ TickerSpeed� �� *FindAncestor+$ local:CTickerL�.��U� $ txtblkTicker2��$	 Opacity�.\�n� $ 0��$ 0Q��� $ Forever�� `� 8   $ 0��.1��� $ ActualWidth�$ 
cnvsTicker���� $ ActualWidth�$ txtblkTicker1���� $ TickerSpeed� �� *FindAncestor+$ local:CTickerL� `� 8  .1��� $ ActualWidth�$ 
cnvsTicker���� $ ActualWidth�$ txtblkTicker1���� $ TickerSpeed� �� *FindAncestor+$ local:CTickerL�.��W� $ txtblkTicker2��$	 (Canvas.Left)�
 �� *ActualWidth+$ 
cnvsTicker�� �� *ActualWidth+$ txtblkTicker1��    `� 8  $ 1��.1��� $ ActualWidth�$ 
cnvsTicker���� $ ActualWidth�$ txtblkTicker1���� $ TickerSpeed� �� *FindAncestor+$ local:CTickerL�.���� -    ��Name$ 
cnvsTicker��
������ $��Stretch=�$��Stretch=�$	��100��$��0q�}�. .+��� -   $��txtblkTickerOneTime���� -   $��txtblkTicker1���� -   $��txtblkTicker2��y��� *RunAnimation+ A   ;DBManager.Global.Converters.BoolToVisibilityMarkupConverter  �� 
����  �$��10��$y�	Collapsed=� �� Left �� *ActualWidth+$ 
cnvsTicker�� 4   .DBManager.InterfaceElements.HalfWidthConverter  ���� *ActualHeight+$ 
cnvsTicker��