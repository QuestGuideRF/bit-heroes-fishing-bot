using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using System.ComponentModel;
namespace fisher {
	public partial class Form1 : Form {
		#region Declarations
		private static bool steam;
		private static Point locationStartButton;
		private static Point locationCloseShellDialogBox;
		private static Point locationTradeFishButton;
		private static Point locationCloseItGotAwayButton;
		private static Point locationTimerCaughtFish;
		private static Point locationJunkItem;
		private static Point locationTopLeftWeightScreenshot;
		private static Point locationBottomRightWeightScreenshot;
		private static Point location100Position;
		Color startButtonGreen = Color.FromArgb(155, 208, 30);
		Color castButtonBlue = Color.FromArgb(030, 170, 208);
		Color colorCloseItGotAwayButton = Color.FromArgb(030, 170, 208);
		Color colorTimerCaughtFishKong = Color.FromArgb(56, 255, 56);
		Color colorTimerCaughtFishSteam = Color.FromArgb(59, 255, 59);
		Color colorJunkItem = Color.FromArgb(255, 255, 255);
		Color oneHundredCatchColor = Color.FromArgb(77, 254, 0);
		MethodHelper helper;
		#endregion
		public Form1() {
			InitializeComponent();
			backgroundThread.WorkerReportsProgress = true;
			backgroundThread.WorkerSupportsCancellation = true;
			backgroundThreadGetTimes.WorkerReportsProgress = true;
			backgroundThreadGetTimes.WorkerSupportsCancellation = true;
			kongButton.CheckedChanged += new EventHandler(platform_CheckedChanged);
			kartridgeButton.CheckedChanged += new EventHandler(platform_CheckedChanged);
			steamButton.CheckedChanged += new EventHandler(platform_CheckedChanged);
			castCatchLocationLbl.Text = "Местоположение заброса/лова:\nНажмите кнопку «Старт» на экране начала рыбалки.";
			helper = new MethodHelper(steam);
			baitToUseText.Enabled = false;
			findLocationBtn.Enabled = false;
			autoBtn.Enabled = false;
			cancelAutoModeBtn.Enabled = false;
		}
		private void platform_CheckedChanged(object sender, EventArgs e) {
			RadioButton radioButton = sender as RadioButton;
			if (kongButton.Checked || kartridgeButton.Checked) {
				steam = false;
			} else if (steamButton.Checked) {
				steam = true;
			}
			baitToUseText.Enabled = true;
			findLocationBtn.Enabled = true;
		}
		private void CastCatchLocation_Click(object sender, EventArgs e) {
			locationStartButton = helper.FindColor(startButtonGreen);
			if (locationStartButton == new Point()) {
				MessageBox.Show("Не удалось найти кнопку «Пуск».\nУбедитесь, что вы находитесь на экране начала рыбалки.\nЕсли вы считаете, что это может быть ошибка, отправьте сообщение о проблеме на github.", "Кнопка «Пуск» не найдена", MessageBoxButtons.OK, MessageBoxIcon.Error);
			} else {
				locationStartButton = helper.getScreenLocationPoint(locationStartButton);
				if (steam) {
					locationTradeFishButton = new Point(locationStartButton.X, locationStartButton.Y - 40);
					locationCloseShellDialogBox = new Point(locationStartButton.X + 270, locationStartButton.Y - 350);
					locationCloseItGotAwayButton = new Point(locationStartButton.X + 20, locationStartButton.Y - 130);
					locationTimerCaughtFish = new Point(locationStartButton.X - 200, locationStartButton.Y - 70);
					locationJunkItem = new Point(locationStartButton.X + 40, locationStartButton.Y - 182);
					locationTopLeftWeightScreenshot = new Point(locationStartButton.X - 25, locationStartButton.Y - 130);
					locationBottomRightWeightScreenshot = new Point(locationStartButton.X + 160, locationStartButton.Y - 50);
					location100Position = new Point(locationStartButton.X + 370, locationStartButton.Y - 81);
				} else {
					locationTradeFishButton = new Point(locationStartButton.X, locationStartButton.Y - 15);
					locationCloseShellDialogBox = new Point(locationStartButton.X + 265, locationStartButton.Y - 325);
					locationCloseItGotAwayButton = new Point(locationStartButton.X + 30, locationStartButton.Y - 100);
					locationTimerCaughtFish = new Point(locationStartButton.X - 200, locationStartButton.Y - 70);
					locationJunkItem = new Point(locationStartButton.X + 100, locationStartButton.Y - 155);
					locationTopLeftWeightScreenshot = new Point(locationStartButton.X - 25, locationStartButton.Y - 130);
					locationBottomRightWeightScreenshot = new Point(locationStartButton.X + 160, locationStartButton.Y - 50);
					location100Position = new Point(locationStartButton.X + 373, locationStartButton.Y - 81);
				}
				castCatchLocationLbl.Text = "Место применения/ловли:\n" + locationStartButton.ToString();
				autoBtn.Enabled = true;
				cancelAutoModeBtn.Enabled = true;
			}
		}
		private void autoBtn_Click(object sender, EventArgs e) {
			backgroundThread.RunWorkerAsync();
		}
		private void backgroundThread_DoWork(object sender, DoWorkEventArgs e) {
			BackgroundWorker worker = sender as BackgroundWorker;
			int baitToUse = int.Parse(baitToUseText.Text);
			int i = 0;
			int baitUsed = 0;
			while (i < baitToUse) {
				if (worker.CancellationPending == true) {
					e.Cancel = true;
					break;
				} else {
					bool caughtFish = true;
					bool fishGetAway = true;
					printMessage(baitUsed, baitToUse, " использована наживка.\nВыполняем заброс.");
					++baitUsed;
					Invoke(new Action(() => Refresh()));
					Invoke(new Action(() => helper.startCast(locationStartButton)));
					Invoke(new Action(() => Cursor.Position = locationTimerCaughtFish));
					while (caughtFish) {
						if (worker.CancellationPending == true) {
							e.Cancel = true;
							break;
						}
						printMessage(baitUsed, baitToUse, " использована наживка.\nОжидание результата заброса.");
						Invoke(new Action(() => Refresh()));
						Color color = helper.GetPixelColor(locationTimerCaughtFish);
						if (helper.AreColorsSimilar(color, colorTimerCaughtFishKong, 20)) {
							if (worker.CancellationPending == true) {
								e.Cancel = true;
								break;
							}
							printMessage(baitUsed, baitToUse, " использована наживка.\nВыполнение ловли.");
							Invoke(new Action(() => Refresh()));
							Invoke(new Action(() => helper.catchFish(location100Position, oneHundredCatchColor)));
							Thread.Sleep(5000);
							while (fishGetAway) {
								if (worker.CancellationPending == true) {
									e.Cancel = true;
									break;
								}
								if (helper.GetPixelColor(locationTradeFishButton) == startButtonGreen) {
									printMessage(baitUsed, baitToUse, " использована наживка.\nПойман.");
									Invoke(new Action(() => Refresh()));
									Invoke(new Action(() => helper.tradeItemThenCloseClick(locationTradeFishButton, locationCloseShellDialogBox)));
									fishGetAway = false;
								}
								else if (helper.GetPixelColor(locationCloseItGotAwayButton) == colorCloseItGotAwayButton) {
									printMessage(baitUsed, baitToUse, " использована наживка.\nРыба ушла. Извини :(");
									Invoke(new Action(() => Refresh()));
									helper.fishGotAwayClick(locationCloseItGotAwayButton);
									fishGetAway = false;
								}
							}
							caughtFish = false;
						}
						else if (helper.GetPixelColor(locationJunkItem) == colorJunkItem) {
							printMessage(baitUsed, baitToUse, " использована наживка.\nПойман мусор");
							Invoke(new Action(() => Refresh()));
							Invoke(new Action(() => helper.tradeItemThenCloseClick(locationTradeFishButton, locationCloseShellDialogBox)));
							caughtFish = false;
						}
					}
					++i;
				}
			}
			printMessage(baitUsed, baitToUse, " наживка использована.\nГотово.");
			backgroundThread.CancelAsync();
		}
		private void cancelAutoModeBtn_Click(object sender, EventArgs e) {
			backgroundThread.CancelAsync();
		}
		private void printMessage(int baitUsed, int baitToUse, string msg) {
			debugAutoStepLbl.Invoke((MethodInvoker)delegate {
				debugAutoStepLbl.Text = baitUsed + "/" + baitToUse + msg;
			});
		}
	}
}