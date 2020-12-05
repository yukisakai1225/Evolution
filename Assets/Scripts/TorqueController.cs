using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UniRx;
using UnityEngine;

[RequireComponent(typeof(MoveTorque))]
public class TorqueController : MonoBehaviour{

  private float _interval = 0.1f;
  private Queue<float> _queue = new Queue<float>();
  [SerializeField] [NotNull] public LegModelMonoBehaviour LegModelMonoBehaviour;

  private void Start(){
    foreach (float dna in LegModelMonoBehaviour.LegModel.DNA)
    {
      _queue.Enqueue(dna);
    }

    Observable
	        .Timer(TimeSpan.FromSeconds(0), TimeSpan.FromSeconds(_interval))
	        .Where(_ => _queue.Count > 0)
	        .Subscribe(_ => {

	            LegModelMonoBehaviour.LegModel.MoveTorque.Magnitude = _queue.Dequeue();

	        }).AddTo(gameObject);
  }

}
