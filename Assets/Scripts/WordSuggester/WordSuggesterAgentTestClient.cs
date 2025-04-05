using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WordSuggesterAgentTestClient : MonoBehaviour
{
    [SerializeField] private Button _wordsButton;
    [SerializeField] private Button _sentenceButton;
    [SerializeField] private Button _batchButton;
    [SerializeField] private TMP_Dropdown _categoryDropdown;
    [SerializeField] private TMP_Dropdown _toLanguageDropdown;
    [SerializeField] private InputField _inputField;
    [SerializeField] private WordSuggesterAgent _agent;
    [SerializeField] private TMP_Text _textField;

    private String[] specialSceneWordsEnglish = new String[]
    {
        "Picture",
        "Window",
        "Storage",
        "Table",
        "Plant",
        "Couch",
        "Screen",
        "Bed",
        "Door",
        "Lamp"
    };

    private String[] languages = new String[]
    {
        "Spanish",
        "Hungarian",
        "French",
        "Bulgarian"
    };
    
    // Start is called before the first frame update
    void Start()
    {
        _wordsButton.onClick.AddListener(OnWordsClick);
        _sentenceButton.onClick.AddListener(OnSentenceClick);
        _batchButton.onClick.AddListener(OnBatchClick);
    }

    private void AppendMessage(String message)
    {
        Debug.Log($"{message}");
        _textField.text += message + "\n";
    }

    async Task ProcessWordsQueryAsync(string fromWord, string fromLanguage, 
        string toLanguage, StreamWriter writer = null)
    {
        WordSuggesterAgent.WordCategory categoryEnum = 
            (WordSuggesterAgent.WordCategory) Enum.Parse(typeof(WordSuggesterAgent.WordCategory), 
                _categoryDropdown.value.ToString());

        var result =  await 
            _agent.FindRelatedWords(fromWord, 
                fromLanguage, toLanguage, 
                categoryEnum);
        var relatedWords = result;
        
        // Iterate through the list and print each column

        string fileStr = $"{fromWord}\t{toLanguage}\t";        
        foreach (var tuple in relatedWords)
        {
            // Access the first column of the tuple
            string column1 = tuple.Item1;
            // Access the second column of the tuple
            string column2 = tuple.Item2;

            // Print each column
            string res = $"{column1} -> {column2}";
            AppendMessage(res);

            fileStr += $"{res}\t";
        }

        if (writer != null)
        {
            writer.WriteLine(fileStr);
        }
        
        if (relatedWords.Count == 0)
        {
            AppendMessage("empty/non-parseable response");
        }
    }
    
    async Task ProcessSentenceQueryAsync(string fromWord, string fromLanguage, 
        string toLanguage, StreamWriter writer = null)
    {
        var result = await
            _agent.ConstructSentence(fromWord,
                toLanguage, fromLanguage);
        var sentences = result;
        
        // Iterate through the list and print each column
        // Print each column
        AppendMessage(sentences.Item1);
        AppendMessage(sentences.Item2);

        string fileStr = $"{fromWord}\t{toLanguage}\t{sentences.Item1}\t{sentences.Item2}";
        if (writer != null)
        {
            writer.WriteLine(fileStr);
        }
    }

    void OnWordsClick()
    {
        _textField.text = "";
        var toLanguage = _toLanguageDropdown.options[_toLanguageDropdown.value].text;

        ProcessWordsQueryAsync(_inputField.text, "English", toLanguage);
    }

    void OnSentenceClick()
    {
        _textField.text = "";
        var toLanguage = _toLanguageDropdown.options[_toLanguageDropdown.value].text;

        ProcessSentenceQueryAsync(_inputField.text, "English", toLanguage);
    }

    async void OnBatchClick()
    {
        string filePath = "wordsSuggester_batch.txt";
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }
        foreach (var toLanguage in languages)
        {
//        var toLanguage = _toLanguageDropdown.options[_toLanguageDropdown.value].text;
            // Open the file in append mode and create if it doesn't exist
            using (StreamWriter writer = new StreamWriter(filePath, true))
            {
                foreach (var word in specialSceneWordsEnglish)
                {
                    //await ProcessSentenceQueryAsync(word, "English", toLanguage, writer);
                    //await ProcessWordsQueryAsync(word, "English", toLanguage, writer);
                }
            }
        }
        Debug.Log("done batch processing");
    }
}
