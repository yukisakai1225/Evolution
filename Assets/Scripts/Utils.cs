using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using SubScripts.Base;
using Random = UnityEngine.Random;

class Utils : SingletonMonobehaviour<Utils> {

    [NotNull] public List<LegModel> LegModels = new List<LegModel>();

    public float eliteRate = 0.2f;
    public int modelNum = 100;
    public int DNALength = 100;
    float epsilon;
    public float SimulationTime = 10f;
    public float OldGenerationRate {get {return Mathf.Max(1 - eliteRate*3);}}
    private int _generationNum = 1;
    public float MaxTorque = 100;
    public float MinTorque = -60;
    [SerializeField] [NotNull] private GameObject _LegModelPrefab;

    public override void Initialization()
    {
      base.Initialization();
      epsilon = Mathf.Sqrt(DNALength + 2);
      Debug.Log(_generationNum + "世代目");

      GenerateModels();

      Observable.Timer(TimeSpan.FromSeconds(0), TimeSpan.FromSeconds(SimulationTime))
            .Delay(TimeSpan.FromSeconds(SimulationTime))// 最初のみこの時間だけ遅らせる
            .Subscribe(_ =>
            {
                // 評価
                getResult();

                // 終了条件

                // 選択
                var eliteModels = selectElite();

                // 交叉
                var crossedModels = crossModels(eliteModels);

                // 生成
                LegModels = generateNewGeneration(LegModels, eliteModels, crossedModels);

                _generationNum++;
                Debug.Log(_generationNum + "世代目");
            }).AddTo(gameObject);

    }

    public void getResult() {
        float bestScore = 0;
        foreach (LegModel legModel in LegModels) {
            Rigidbody rigidBody = legModel.SockerBallRigidBody;
            legModel.Result = rigidBody.transform.position.z;
            if (bestScore < legModel.Result)
              bestScore = legModel.Result;
        }

        Debug.Log("BestScore : " + bestScore);
    }

    private void GenerateModels()
    {
        DestroyModels();

        for (int i=0; i<modelNum; i++)
        {
            GameObject gameObject = Instantiate(_LegModelPrefab, new Vector3(i, 0, 0), Quaternion.identity);

            LegModelMonoBehaviour legModelMonoBehaviour = gameObject.GetComponent<LegModelMonoBehaviour>();

            if (legModelMonoBehaviour == null)
              Debug.Log(i + " : legModelMonoBehaviour is null");

            LegModel legModel = legModelMonoBehaviour.LegModel;

            if (legModel == null)
              Debug.Log(i + " : legModel is null");

            legModel.DNA.Clear();

            for (int j=0; j<DNALength; j++)
            {
                legModel.DNA.Add(Random.Range(MinTorque, MaxTorque));
            }
            // Debug.Log("DNA " + i + " : " + string.Join(", ", legModel.DNA));
            LegModels.Add(legModel);
        }
    }

    private void DestroyModels()
    {
        LegModels.ForEach(i => Destroy(i.LegModelMonoBehaviour.gameObject));
        LegModels.Clear();
    }

    public String toString() {
        String str = "";
        str += "------------------------------------------------\n";
        int i= 0;
        foreach (LegModel leg in LegModels) {
            i++;
            str += leg.toString() + "\n";
        }
        str += "------------------------------------------------\n";

        return str;
    }

    public int compareLegs(LegModel a, LegModel b, Boolean reverse)
    {
        int m = 1;
        if (reverse)
            m = -1;

        if (a.Result - b.Result < 0)
            return -1 * m;
        else if (a.Result - b.Result >0)
            return 1 * m;
        else
            return 0;
    }

    public List<LegModel> selectElite() {
        List<LegModel> eliteModels = new List<LegModel>();
        List<LegModel> tmpModels = LegModels;
        tmpModels.Sort((a, b) => compareLegs(a, b, true));
        for(int i=0; i<eliteRate*modelNum; i++) {
            eliteModels.Add(tmpModels[i]);
        }

        return eliteModels;
    }

    public List<LegModel> crossModels(List<LegModel> eliteModels)
    {
        List<LegModel> newModels = new List<LegModel>();

        for(int i=0; i<eliteRate*2*modelNum; i++) {
            LegModel newModel = new LegModel();
            newModel.DNA = new List<float>();
            for (int j=0; j<DNALength; j++)
            {
                newModel.DNA.Add(0);
            }
            newModels.Add(newModel);
        }


        for(int i=0; i<eliteRate*2*modelNum; i++) {
            List<float> DNA1 = eliteModels.LoopElementAt(i).DNA;
            List<float> DNA2 = eliteModels.LoopElementAt(i+1).DNA;
            List<float> DNA3 = eliteModels.LoopElementAt(i+2).DNA;

            List<float> resDNA1 = newModels.LoopElementAt(i).DNA;
            List<float> resDNA2 = newModels.LoopElementAt(i+1).DNA;
            List<float> resDNA3 = newModels.LoopElementAt(i+2).DNA;

            List<float> gPointList = new List<float>();

            for (int j=0; j<DNALength; j++)
            {
                gPointList.Add((DNA1[j] + DNA2[j] + DNA3[j]) / 3f);
            }

            List<float> sPointList1 = new List<float>();
            List<float> sPointList2 = new List<float>();
            List<float> sPointList3 = new List<float>();

            for (int j=0; j<DNALength; j++)
            {
                sPointList1.Add(gPointList[j] + epsilon * (DNA1[j] - gPointList[j]));
                sPointList2.Add(gPointList[j] + epsilon * (DNA2[j] - gPointList[j]));
                sPointList3.Add(gPointList[j] + epsilon * (DNA3[j] - gPointList[j]));
            }

            for (int j=0; j<DNALength; j++)
            {
                float r1 = Random.Range(0, 1f);
                float r2 = Mathf.Pow(Random.Range(0, 1f), 0.5f);

                resDNA1[j] = sPointList1[j];
                resDNA2[j] = r1 * (sPointList1[j] - sPointList2[j]) + sPointList2[j];
                resDNA3[j] = r2 * (sPointList2[j] - sPointList3[j] + r1 * (sPointList1[j] - sPointList2[j]) + sPointList3[j]);
            }
        }

        foreach (LegModel legModel in newModels) {
            for (int i=0; i<DNALength; i++) {
                legModel.DNA[i] = Mathf.Clamp((float)legModel.DNA[i], MinTorque, MaxTorque);
            }
        }

        return newModels;
    }

    private List<LegModel> generateNewGeneration(List<LegModel> currentModels, List<LegModel> eliteModels, List<LegModel> crossedModels)
    {
        List<LegModel> nextModels = new List<LegModel>();

        // エリート遺伝子の受け継ぎ
        foreach (LegModel legModel in eliteModels)
        {
            nextModels.Add(Instantiate(_LegModelPrefab, new Vector3(nextModels.Count(), 0, 0), Quaternion.identity).GetComponent<LegModelMonoBehaviour>().LegModel);

            nextModels.Last().DNA = legModel.DNA;

            nextModels.Last().Evaluation = legModel.Evaluation;

            nextModels.Last().LegModelMonoBehaviour.gameObject.name = "Elite";
        }

        // 子孫遺伝子の受け継ぎ
        foreach (LegModel legModel in crossedModels)
        {
            nextModels.Add(Instantiate(_LegModelPrefab, new Vector3(nextModels.Count(), 0, 0), Quaternion.identity).GetComponent<LegModelMonoBehaviour>().LegModel);

            nextModels.Last().DNA = legModel.DNA;

            nextModels.Last().Evaluation = legModel.Evaluation;

            nextModels.Last().LegModelMonoBehaviour.gameObject.name = "Crossed";
        }

        // 現行遺伝子の受け継ぎ
        foreach (LegModel legModel in currentModels.Take((int)(modelNum * OldGenerationRate)))
        {
            nextModels.Add(Instantiate(_LegModelPrefab, new Vector3(nextModels.Count(), 0, 0), Quaternion.identity).GetComponent<LegModelMonoBehaviour>().LegModel);

            nextModels.Last().DNA = legModel.DNA;

            nextModels.Last().Evaluation = legModel.Evaluation;

            nextModels.Last().LegModelMonoBehaviour.gameObject.name = "Old";
        }

        foreach (LegModel legModel in LegModels)
        {
            Destroy(legModel.LegModelMonoBehaviour.gameObject);
        }

        LegModels.Clear();

        return nextModels;
    }
}
