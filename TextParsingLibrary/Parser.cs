using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;

namespace TextParsingLibrary
{
    public class Parser
    {
        private FileInfo fileInfo;
        private Encoding encoding;
        int numCharactersParsed;
        private StringBuilder sb = new StringBuilder();

        public Parser(string filePath, Encoding encoding)
        {
            fileInfo = new FileInfo(filePath);
            this.encoding = encoding;
        }

        public Parser(Encoding encoding)
        {
            this.encoding = encoding;
        }

        /// <summary>
        /// This function is used as a callback function for an externally created BackgroundWorker to 
        /// support the parsing of large text files in the background.
        /// The result in the form of a TextStatistic object is passed in e.Result and can be retreived in 
        /// the RunWorkerCompletedEventHandler with e.Result.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Worker_DoParse(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            TextStatistics stats = new TextStatistics();

            numCharactersParsed = 0;
            int oldRelativeProgress = 0;
            int newRelativeProgress = 0;
            using (StreamReader reader = new StreamReader(fileInfo.FullName, encoding))
            {
                StringBuilder sb = new StringBuilder();
                while (reader.EndOfStream == false)
                {
                    if (worker.CancellationPending == true)
                    {
                        e.Cancel = true;
                        break;
                    }

                    ParseNextChar(reader, stats);

                    newRelativeProgress = (int)(((float)numCharactersParsed / (float)fileInfo.Length) * 100.0);

                    // only update progress if it changed by more than 1%, otherwise the other tasks
                    // will be interrupted too frequently and a GUI may become weakly responsive.
                    if (oldRelativeProgress < newRelativeProgress)
                    {
                        worker.ReportProgress(newRelativeProgress);
                        oldRelativeProgress = newRelativeProgress;
                    }
                }

            }

            e.Result = stats;
        }

        /// <summary>
        /// This method parses the given text file in a blocking fashion.
        /// ATTENTION: large text files should be parsed using a BackgroundWorker an the 
        /// Worker_DoParse() method.
        /// </summary>
        /// <returns>a TextStatistics object containig the statistics of the parsed file</returns>
        public TextStatistics Parse()
        {
            TextStatistics stats = new TextStatistics();
            numCharactersParsed = 0;
            using (StreamReader reader = new StreamReader(fileInfo.FullName, encoding))
            {
                sb.Clear();
                while (reader.EndOfStream == false)
                {
                    ParseNextChar(reader, stats);
                }

            }
            return stats;
        }

        /// <summary>
        /// Internally used function to parse the next character from the reader and update the stats.
        /// Furthermore it adds a word to the WordFrequency dictionary if a new word has been detected.
        /// The external 
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="sb"></param>
        /// <param name="stats"></param>
        private void ParseNextChar(StreamReader reader, TextStatistics stats)
        {
            char c = (char)reader.Read();

            if (c < 0)
            {
                return;
            }

            numCharactersParsed++;

            if (c != ' ' && c != '\r' && c != '\n')
            {
                // no new word found
                sb.Append(c);
            }
            else if (sb.Length > 0)
            {
                // end of new word found
                stats.WordCount++;
                stats.IncrementWordFrequency(sb.ToString());
                sb.Clear();
            }
        }

        /// <summary>
        /// Sets the encoding used to parse the file.
        /// Encoding: "For ANSI encodings, the best fit behavior is the default." [1]
        // [1] https://docs.microsoft.com/en-us/dotnet/api/system.text.encoding?view=netframework-4.7.2
        /// </summary>
        /// <param name="encoding"></param>
        public void SetEncoding(Encoding encoding)
        {
            this.encoding = encoding;
        }

        /// <summary>
        /// Opens the file specified in filePath and checks if the file exists.
        /// If the file does not exist, the FileNotFoundException is thrown.
        /// </summary>
        /// <param name="filePath"></param>
        /// <exception cref="FileNotFoundException"></exception>
        public void SetTextFile(string filePath)
        {
            fileInfo = new FileInfo(filePath);
            if (fileInfo.Exists == false)
            {
                throw new FileNotFoundException();
            }
        }
    }
}
