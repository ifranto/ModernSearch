using Lucene.Net.Analysis;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Store;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernSearch
{
    public class Search
    {
        private FSDirectory _directory = FSDirectory.Open("C:/PathToApplication/App_Data/Lucene");

        public void Code()
        {
            Analyzer _keywordanalyzer = new KeywordAnalyzer();
            Analyzer _simpleanalyzer = new Lucene.Net.Analysis.SimpleAnalyzer();
            Analyzer _stopanalyzer = new Lucene.Net.Analysis.StopAnalyzer(Lucene.Net.Util.Version.LUCENE_30);
            Analyzer _whitespaceanalyzer = new Lucene.Net.Analysis.WhitespaceAnalyzer();
            Analyzer _standardanalyzer = new Lucene.Net.Analysis.Standard.StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30);


            var _perfieldanalyzer = new Lucene.Net.Analysis.PerFieldAnalyzerWrapper(_standardanalyzer);

            _perfieldanalyzer.AddAnalyzer("firstname", _keywordanalyzer);
            _perfieldanalyzer.AddAnalyzer("lastname", _keywordanalyzer);


            IndexWriter _writer = new IndexWriter(_directory, _perfieldanalyzer, IndexWriter.MaxFieldLength.UNLIMITED);

            IndexReader _reader = _writer.GetReader();

            IndexSearcher _searcher = new IndexSearcher(_reader);


            //QueryParser parser = new QueryParser(Lucene.Net.Util.Version.LUCENE_30, "title", _standardanalyzer);

            string[] fields = new[] { "text", "title", "author" };
            var boosts = new Dictionary<string, float>();
            boosts.Add("text", 2.0f);
            boosts.Add("title", 1.5f);
            QueryParser parser = new MultiFieldQueryParser(Lucene.Net.Util.Version.LUCENE_30, fields, _standardanalyzer, boosts);
            Query query = parser.Parse("lucene is great");


            TopDocs hits = _searcher.Search(query, 1000);

            IEnumerable<Document> docs = hits.ScoreDocs.Select(hit => _searcher.Doc(hit.Doc));

            var books = docs.Select(doc => new Book()
                        {
                            Text = doc.Get("text"),
                            Title = doc.Get("title"),
                            Author = doc.Get("author"),
                            Length = Int32.Parse(doc.Get("length"))
                        });


            _writer.Optimize();
            _writer.Commit();
            _writer.DeleteAll();


        }

        //public void Delete(string id)
        //{
        //    Analyzer _standardanalyzer = new Lucene.Net.Analysis.Standard.StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30);
        //    IndexWriter _writer = new IndexWriter(_directory, _standardanalyzer, IndexWriter.MaxFieldLength.UNLIMITED);

        //    QueryParser parser = new QueryParser(Lucene.Net.Util.Version.LUCENE_30, "id", _standardanalyzer);
        //    Query query = parser.Parse(id);

        //    _writer.DeleteDocuments(query);
        //}

        public void Delete(string id)
        {
            Analyzer _standardanalyzer = new Lucene.Net.Analysis.Standard.StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30);
            IndexWriter _writer = new IndexWriter(_directory, _standardanalyzer, IndexWriter.MaxFieldLength.UNLIMITED);

            _writer.DeleteDocuments(new Term("id", id));
        }

        public class Book
        {
            public string Text { get; set; }
            public string Title { get; set; }
            public string Author { get; set; }
            public int Length { get; set; }

        }
    }
}
