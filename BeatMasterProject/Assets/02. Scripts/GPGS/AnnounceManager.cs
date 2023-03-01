using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class AnnounceManager : MonoBehaviour
{
    [SerializeField] private Text _text;
    [SerializeField] private string _url;
    [SerializeField] private Toggle _toggle;
    [SerializeField] private GameObject _panel;
    private PlayerData _playerData;

    public void Awake()
    {
        DataCenter.Instance.LoadData();
        _playerData = DataCenter.Instance.GetPlayerData();
    }

    private void Start()
    {
        Announce();
        if (_playerData.checkAnnounce)
        {
            _toggle.gameObject.SetActive(false);
        }
        else
        {
            UIManager.instance.OpenPanel(_panel);
        }
        _toggle.onValueChanged.AddListener((bool bOn) =>
        {
            DataCenter.Instance.UpdateCheckAnnounce(bOn);
        });
    }

    async void Announce()
    {
        _url = "https://dev-shim.tistory.com/36";

        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(_url);
        HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync();
        Stream stream = response.GetResponseStream();
        StreamReader reader = new StreamReader(stream);

        string html = await reader.ReadToEndAsync();
        response.Close();
        reader.Close();
        stream.Close();

        Regex regex = new Regex(@"<div\b[^>]*>(.*?)</div>");
        MatchCollection matches = regex.Matches(html);

        StringBuilder sb = new StringBuilder();
        foreach (Match match in matches)
        {
            string contents = match.Groups[1].Value;

            Regex regex1 = new Regex(@"<[^>]+>([^<]+)<\/[^>]+>");
            string result = regex1.Match(contents).Groups[1].Value;
            sb.Append(result + " ");
        }
        _text.text = sb.ToString();
    }
}
