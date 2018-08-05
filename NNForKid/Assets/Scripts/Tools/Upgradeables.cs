using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UpgradeableRecordHost {
	public List<Upgradeable> clients = new List<Upgradeable>();
	public List<UpgradeableRecord> records = new List<UpgradeableRecord>();

	public void Subscribe(Upgradeable upgradeable) {
		clients.Add(upgradeable);

		foreach (var record in records) {
			upgradeable.RecordAdded(record);
		}
	}

	public void Unsubscribe(Upgradeable upgradeable) {
		clients.Remove(upgradeable);
	}

	public void Publish(UpgradeableRecord record) {
		records.Add(record);
		foreach (var client in clients) {
			client.RecordAdded(record);
		}
	}

	public void Unpublish(UpgradeableRecord record) {
		records.Remove(record);
		foreach (var client in clients) {
			client.RecordRemoved(record);
		}
	}
}

public enum UpgradeMethod {
	Add,
	Multiply,
	Replace
}

//######################
/// Upgradeables       # 
//######################

[Serializable]
public abstract class Upgradeable : IEquatable<Upgradeable> {
	public string key;

	public abstract void RecordAdded(UpgradeableRecord record);
	public abstract void RecordRemoved(UpgradeableRecord record);

	public bool Equals(Upgradeable other) {
		if (ReferenceEquals(null, other)) return false;
		return ReferenceEquals(this, other) || string.Equals(key, other.key);
	}

	public override bool Equals(object obj) {
		if (ReferenceEquals(null, obj)) return false;
		if (ReferenceEquals(this, obj)) return true;
		return obj.GetType() == GetType() && Equals((Upgradeable) obj);
	}

	public override int GetHashCode() {
		return (key != null ? key.GetHashCode() : 0);
	}
}

[Serializable]
public class UpgradeableFloat : Upgradeable {
	public float value;

	private bool m_cacheValid;
	private float m_cachedValue;

	private float currentValue {
		get {
			if (!m_cacheValid) Calculate();
			return m_cachedValue;
		}
	}

	private readonly List<FloatUpgradeableRecord> m_upgrades = new List<FloatUpgradeableRecord>();


	public UpgradeableFloat(float v) {
		value = v;
	}

	public void Calculate() {
		var temp = value;

		var adds = m_upgrades.Where(x => x.method == UpgradeMethod.Add).ToList();
		temp += adds.Sum(o => o.value);

		var muls = m_upgrades.Where(x => x.method == UpgradeMethod.Multiply).ToList();
		temp = muls.Aggregate(temp, (current, o) => current * o.value);

		var reps = m_upgrades.Where(x => x.method == UpgradeMethod.Replace).ToList();
		if(reps.Count > 0) temp = reps[reps.Count - 1].value;

		m_cachedValue = temp;
		m_cacheValid = true;
	}

	public void Apply(FloatUpgradeableRecord info) {
		m_upgrades.Add(info);
		m_cacheValid = false;
	}

	public void Remove(string id) {
		m_upgrades.RemoveAt(m_upgrades.IndexOf(m_upgrades.SingleOrDefault(x => x.key == id)));
		m_cacheValid = false;
	}

	public override void RecordAdded(UpgradeableRecord record) {
		if(record.BindingKey() != key) return;
		Apply((FloatUpgradeableRecord)record);
	}

	public override void RecordRemoved(UpgradeableRecord record) {
		if(record.BindingKey() != key) return;
		Remove(record.BindingKey());
	}
	
	public static implicit operator float(UpgradeableFloat uf) {
		return uf.currentValue;
	}

	public static implicit operator UpgradeableFloat(float f) {
		return new UpgradeableFloat(f);
	}
}

[Serializable]
public class UpgradeableAnimationCurve : Upgradeable{
	public AnimationCurve value = AnimationCurve.Linear(0,0,1,1);

	private bool m_cacheValid;
	private AnimationCurve m_cachedValue = AnimationCurve.Linear(0,0,1,1);

	private AnimationCurve currentValue {
		get {
			if (!m_cacheValid) Calculate();
			return m_cachedValue;
		}
	}

	private readonly List<AnimationCurveUpgradeableRecord> m_upgrades = new List<AnimationCurveUpgradeableRecord>();

	public UpgradeableAnimationCurve() {
		value = AnimationCurve.Linear(0,0,1,1);
		m_upgrades = new List<AnimationCurveUpgradeableRecord>();
	}

	public UpgradeableAnimationCurve(AnimationCurve v) {
		value = v;
		m_upgrades = new List<AnimationCurveUpgradeableRecord>();
	}

	public void Calculate() {
		var temp = value;
		if(m_upgrades.Count > 0) temp = m_upgrades[m_upgrades.Count - 1].value;

		m_cachedValue = temp;
		m_cacheValid = true;
	}

	public void Apply(AnimationCurveUpgradeableRecord info) {
		m_upgrades.Add(info);
		m_cacheValid = false;
	}

	public void Remove(string id) {
		m_upgrades.RemoveAt(m_upgrades.IndexOf(m_upgrades.SingleOrDefault(x => x.key == id)));
		m_cacheValid = false;
	}

	public override void RecordAdded(UpgradeableRecord record) {
		if(record.BindingKey() != key) return;
		Apply((AnimationCurveUpgradeableRecord)record);
	}

	public override void RecordRemoved(UpgradeableRecord record) {
		if(record.BindingKey() != key) return;
		Remove(record.BindingKey());
	}
	
	public static implicit operator AnimationCurve(UpgradeableAnimationCurve uf) {
		return uf.currentValue;
	}

	public static implicit operator UpgradeableAnimationCurve(AnimationCurve f) {
		return new UpgradeableAnimationCurve(f);
	}
}

//######################
/// UpgradeableRecords # 
//######################
[Serializable]
public class UpgradeableRecord : IEquatable<UpgradeableRecord> {
	public string key;

	public string BindingKey() {
		return key;
	}

	public bool Equals(UpgradeableRecord other) {
		if (ReferenceEquals(null, other)) return false;
		return ReferenceEquals(this, other) || string.Equals(key, other.key);
	}

	public override bool Equals(object obj) {
		if (ReferenceEquals(null, obj)) return false;
		if (ReferenceEquals(this, obj)) return true;
		return obj.GetType() == GetType() && Equals((UpgradeableRecord) obj);
	}

	public override int GetHashCode() {
		return key != null ? key.GetHashCode() : 0;
	}
}

[Serializable]
public class FloatUpgradeableRecord : UpgradeableRecord {
	public UpgradeMethod method;
	public float value;
}

[Serializable]
public class AnimationCurveUpgradeableRecord : UpgradeableRecord {
	public AnimationCurve value;
}