//using SARS;
//using System;
//using System.Windows.Forms;

//public class SZProgress : SevenZip.ICodeProgress
//{
//    public ulong maxSize;
//    public float prog;
//    private HotswapConsole _hotSwap;

//    public SZProgress(HotswapConsole hotSwapConsole)
//    {
//        maxSize = 0;
//        prog = 0.0f;
//        _hotSwap = hotSwapConsole;
//    }

//    public void SetProgress(ulong inSize)
//    {
//        float pgs = (float)inSize / maxSize;
//        if (pgs > prog + 0.005f)
//        {
//            prog = pgs;
//            Console.Write($"\rProgress: %{prog * 100}");
//            SafeProgress(_hotSwap.pbProgress, Convert.ToInt32((int)Math.Round(prog * 100)));
//        }
//    }

//    private static void SafeProgress(ProgressBar progress, int value)
//    {
//        if (progress.InvokeRequired)
//        {
//            progress.Invoke((MethodInvoker)delegate
//            {
//                progress.Value = value;
//            });
//        }
//    }

//    public void SetMaxSize(ulong maxSize)
//    {
//        this.maxSize = maxSize;
//    }

//    public void Clear()
//    {
//        maxSize = 0;
//        prog = 0.0f;
//    }

//    public void AddProgress(ulong delta)
//    {
        
//    }
//}