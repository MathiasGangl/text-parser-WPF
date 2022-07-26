# text-parser-WPF
Simple text parser written in C# as a WPF application.


This WPF application is based on Microsoft's [Model-View-ViewModel Design Pattern](https://docs.microsoft.com/en-us/archive/msdn-magazine/2009/february/patterns-wpf-apps-with-the-model-view-viewmodel-design-pattern). The View is represented by the [MainWindow](https://github.com/MathiasGangl/text-parser-WPF/blob/main/TextParserApp/MainWindow.xaml), the ViewModel can be found [here](https://github.com/MathiasGangl/text-parser-WPF/tree/main/TextParserApp/ViewModel) and the Model including the data model and the business logic can be found in the separate C# library [TextParsingLibrary](https://github.com/MathiasGangl/text-parser-WPF/tree/main/TextParsingLibrary).

I implemented a C# Library (TextParsingLibrary) in a separate project to be compiled into a DLL.
Therefore, to avoid issues with the association between the projects, only open the [Visual Studio Solution File](https://github.com/MathiasGangl/text-parser-WPF/blob/main/TextParserApp/TextParserApp.sln).
Visual Studio will then open both projects in the same Solution Explorer. Otherwise, when opening with the “Open Existing Project” Dialog from Visual Studio I have experienced some issues, that the library could not be found. 


A pre-compiled version as .exe file can be found [here](https://github.com/MathiasGangl/text-parser-WPF/tree/main/Executeable).


![Screenshot of the Text Parser in action.](https://github.com/MathiasGangl/text-parser-WPF/blob/main/Figures/screenshot.jpg)
