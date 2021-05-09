using System;
using System.IO;

namespace courser {
				class ConfigProvider {
								private static ConfigProvider instance;
								private static readonly string configFilePath = Path.Combine(@"./Resources/protected", "config.txt");

								private static byte imgFolderIdx = 0;
								private static byte imgIdx = 1;
								private static byte maxLinesCountIdx = 2;
								private static byte greetingIdx = 3;

								private static string defaultGreeting = "%username%";

								private static string defaultImage = "cactus.jpg";
								private static string imagesFolder = @"./Resources/imgs";
								private static string backgroundImg = defaultImage;
								private static byte maxLines = 5;

								public static readonly char Separator = '|';
								public static readonly int ErrorValue = 20;
								public static readonly byte AllowedAttempts = 3;
								public static readonly string ResourcesFolder = @"./Resources";

								public string ImagesFolder { get => imagesFolder; set { WriteToConfig(imgFolderIdx, value); } }
								public string BackgroundImg {
												get => Path.Combine(imagesFolder, backgroundImg);
												set {
																backgroundImg = value;
																WriteToConfig(imgIdx, value);
												}
								}
								public byte MaxLines { get => (byte) (maxLines - 1); set { WriteToConfig(maxLinesCountIdx, (value).ToString()); } }
								public string Greeting { get => defaultGreeting; set { WriteToConfig(greetingIdx, value); } }

								private ConfigProvider() { }
								public static ConfigProvider getInstance() {
												if(instance == null) {
																instance = new ConfigProvider();
																instance.ReadConfig();
												}
												return instance;
								}

								private void ReadConfig() {
												try {
																string[] configPathes = File.ReadAllLines(configFilePath);
																defaultImage = configPathes[imgIdx];
																backgroundImg = defaultImage;
																maxLines = configPathes.Length > maxLinesCountIdx ? Convert.ToByte(configPathes[maxLinesCountIdx]) : maxLines;
																defaultGreeting = configPathes[greetingIdx];
												}
												catch(FileNotFoundException) {
																CreateConfigFile();
												}
												finally {
																BackgroundImg = defaultImage;
																MaxLines = maxLines;
												}
								}

								private void CreateConfigFile() {
												try {
																string[] config = new string[greetingIdx + 1];
																config[imgFolderIdx] = imagesFolder;
																config[imgIdx] = defaultImage;
																config[maxLinesCountIdx] = maxLines.ToString();
																config[greetingIdx] = defaultGreeting;


																File.WriteAllLines(configFilePath, config);
												}
												catch(Exception) {
																throw new Exception("Cannot create default config");
												}
								}

								private void WriteToConfig(byte idx, string value) {
												try {
																if(File.Exists(configFilePath)) {
																				string[] configPathes = File.ReadAllLines(configFilePath);
																				configPathes[idx] = value;
																				File.WriteAllLines(configFilePath, configPathes);
																}
																else {
																				string[] newConfig = new string[idx + 1];
																				newConfig[idx] = value;

																				File.WriteAllLines(configFilePath, newConfig);
																}
												}
												catch(Exception) {
																throw new Exception("Cannot write value to config");
												}
								}
				}
}
