UI上で `ProgressBar` を使いたい場合、`Parallel.For` の中で直接 UI を更新するのは基本的にNGです。というのも、UI スレッドと並列処理スレッドは別なので、UI 更新は**UI スレッドから行う必要があります**。

---

## ✅ WPFで `ProgressBar` を `Parallel.For` と連携する方法

ここでは **WPF アプリ** を想定したサンプルを紹介します。

### 🧩 基本の構成

1. 並列処理は `Task.Run` や `Parallel.For` で行う  
2. UI更新（`ProgressBar.Value`）は `Dispatcher.Invoke` でUIスレッドに渡す  
3. スレッドセーフなカウントは `Interlocked.Increment`

---

### 🧪 サンプルコード（WPF）

```xaml
<!-- MainWindow.xaml -->
<Window x:Class="ParallelProgressBar.MainWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        Title="Parallel Progress Example" Height="150" Width="400">
    <Grid Margin="10">
        <ProgressBar x:Name="ProgressBar" Height="30" Minimum="0" Maximum="10000" />
    </Grid>
</Window>
```

```csharp
// MainWindow.xaml.cs
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace ParallelProgressBar
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            RunParallelTask();
        }

        private void RunParallelTask()
        {
            int total = 10000;
            int progress = 0;

            Task.Run(() =>
            {
                Parallel.For(0, total, i =>
                {
                    DoSomething(i); // 実際の処理

                    int current = Interlocked.Increment(ref progress);

                    // UIスレッドで ProgressBar を更新
                    Dispatcher.Invoke(() =>
                    {
                        ProgressBar.Value = current;
                    });
                });
            });
        }

        private void DoSomething(int i)
        {
            Thread.Sleep(1); // ダミー処理
        }
    }
}
```

---

### ✅ 補足ポイント

- `Dispatcher.Invoke` は UI スレッドでの操作を保証するためのもの
- `ProgressBar` の `Maximum` を `10000` にして、進捗に合わせて値を更新
- 負荷が大きい場合は `Dispatcher.Invoke` を**一定回数に制限**して更新頻度を下げるのがオススメ（パフォーマンス改善）

---

### 🔧 更新頻度を減らす例（改良版）

```csharp
if (current % 100 == 0 || current == total)
{
    Dispatcher.Invoke(() =>
    {
        ProgressBar.Value = current;
    });
}
```

---

UI が WinForms の場合は `this.Invoke(...)` を使うだけで、ほぼ同じ考え方です。

---

必要なら WinForms 版も書けるし、プログレスバーをもっと「見た目良く」したいとか、「キャンセル」もつけたいとか、そういう応用もできますよ！どうしたいか教えて〜！