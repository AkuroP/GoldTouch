using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using UnityEngine.InputSystem;
using Unity.VisualScripting;
//using UnityEditor.Experimental.GraphView;
using UnityEngine.UI;
using System;
using Random = UnityEngine.Random;
//using UnityEditor;
//using UnityEditor.Experimental.GraphView;
public class GemsManager : MonoBehaviour
{
    private int _nextGem;

    [SerializeField]
    private GameObject _currentGem;
    private Rigidbody _currentGemRb;
    private Vector2 _tapPos;
    public Transform _spawnPoint;
    public float _cooldown = 1f;
    private float _cd;

    [Foldout("Wall"), SerializeField]
    private Transform _wallLeft;
    [Foldout("Wall"), SerializeField]
    private Transform _wallRight;

    [SerializeField]
    private Sprite[] _gemsSprites;
    [SerializeField]
    private Image _nextImage;

    [SerializeField]
    private GameObject[] gemPrefabs;

    [SerializeField] private RectTransform bonusZone;

    private GameObject shopUI;
    // Start is called before the first frame update
    void Start()
    {
        _nextGem = Random.Range(0, 4);
        _currentGem = Instantiate(GameManager.instance.AllGems[_nextGem], _spawnPoint.position, GameManager.instance.AllGems[_nextGem].transform.rotation);
        _currentGemRb = _currentGem.GetComponentInChildren<Rigidbody>();
        _currentGemRb.isKinematic = true;
        _cd = _cooldown;

        _nextGem = Random.Range(0, 4);
        _nextImage.sprite = _gemsSprites[_nextGem];

        AudioManager.instance.PlayRandom(SoundState.SPAWN);
        //_affichageGems[_nextGem].SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.instance.Win) return;
        if (GameManager.instance.CanPlay) return;
        if (_cd > 0) _cd -= Time.deltaTime;
        else
        {
            NextGem();
            GameManager.instance.CanPlay = true;
        }

    }

    public void FusionnerGroupe(List<GameObject> gems)
    {
        if (gems == null || gems.Count != 2)
        {
            Debug.LogWarning("FusionnerGroupe requiert exactement 2 gemmes pour la fusion.");
            return;
        }

        // Prendre les deux gemmes à fusionner
        GameObject olderGem = gems[0];
        GameObject newerGem = gems[1];

        // Vérifier que les gemmes sont valides
        if (olderGem == null || newerGem == null) return;

        if (olderGem.transform.position == _spawnPoint.position || newerGem.transform.position == _spawnPoint.position)
        {
            return;
        }
        // Déplacer la gemme récente vers la position de la gemme plus ancienne
        newerGem.transform.position = olderGem.transform.position;

        Debug.Log("Fusion de deux gemmes par déplacement de la gemme récente sur la plus ancienne !");
    }


    public void NextGem()
    {
        if (GameManager.instance.Win) return;

        // Réinitialise le délai de réapparition
        _cd = _cooldown;

        if (_currentGem == null)
        {
            _currentGem = Instantiate(GameManager.instance.AllGems[_nextGem], _spawnPoint.position, GameManager.instance.AllGems[_nextGem].transform.rotation);

        }
        _currentGemRb = _currentGem.GetComponentInChildren<Rigidbody>();

        // Assigne le tag "NextGem" pour identifier cette gemme

        if (!_currentGemRb.isKinematic) _currentGemRb.isKinematic = true;


        // Prépare la prochaine gemme pour l'affichage suivant
        _nextGem = Random.Range(0, 4);
        _nextImage.sprite = _gemsSprites[_nextGem];
        AudioManager.instance.PlayRandom(SoundState.SPAWN);
    }


    bool ignore;
    public void OnTouchDrag(InputAction.CallbackContext ctx)
    {
        if (GameManager.instance.Win)
            return;

        if (!GameManager.instance.CanPlay)
            return;

        Vector2 screenPoint = ctx.ReadValue<Vector2>();
        if (shopUI == null)
        {
            shopUI = GameObject.Find("Shop"); // Remplacez par le nom de votre GameObject
        }

        if (ctx.started)
        {

            if (RectTransformUtility.RectangleContainsScreenPoint(bonusZone, screenPoint)
                || (shopUI != null && shopUI.activeInHierarchy && RectTransformUtility.RectangleContainsScreenPoint(shopUI.GetComponent<RectTransform>(), screenPoint)))
            {
                Debug.Log("Clic dans la zone de bonus, interaction annulée.");
                ignore = true;
                return;
            }
            ignore = false;
            
        }


        if (ignore) { return; }
        if (ctx.started)
        {
            if (GameManager.instance.CanPlay && _cd <= 0 && !GameManager.instance._inCombo) _currentGem = Instantiate(GameManager.instance.AllGems[_nextGem], _spawnPoint.position, GameManager.instance.AllGems[_nextGem].transform.rotation);
            else return;
        }
        if (_currentGem == null) return;

        _tapPos = Camera.main.ScreenToWorldPoint(ctx.ReadValue<Vector2>());
        if (ctx.performed)
        {
            _tapPos = Camera.main.ScreenToWorldPoint(new Vector3(ctx.ReadValue<Vector2>().x, 0f, 10f));
            _tapPos.y = _spawnPoint.position.y;



            /*if (_tapPos.x > _wallLeft.position.x && _tapPos.x < _wallRight.position.x)*/
            _currentGem.transform.position = new Vector2(_tapPos.x, _tapPos.y);
            if (_tapPos.x <= _wallLeft.position.x) _currentGem.transform.position = _wallLeft.position;
            else if (_tapPos.x >= _wallRight.position.x) _currentGem.transform.position = _wallRight.position;
        }
        if (ctx.canceled)
        {
            if (_currentGem == null)
                return;
            if (_currentGemRb == null)
                _currentGemRb = _currentGem.GetComponentInChildren<Rigidbody>();

            _currentGemRb.isKinematic = false;
            _currentGemRb.AddRelativeTorque(new Vector3(0, 1, 0), ForceMode.Impulse);
            GameManager.instance.IncrementTurn();
            _currentGem = null;
            if (GameManager.instance.CanPlay) GameManager.instance.CanPlay = false;
        }

    }



}
