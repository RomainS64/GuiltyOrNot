using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class SuspectGenerator : MonoSingleton<SuspectGenerator>
{
    [SerializeField] public int minAge;
    [SerializeField] public int maxAgeYoung;
    [SerializeField] public int minAgeOld;
    [SerializeField] public int maxAge;
    [SerializeField] private AnimationCurve ageDistribution;
    
    [SerializeField] public int minSize;
    [SerializeField] public int maxSizeTiny;
    [SerializeField] public int minSizeTall;
    [SerializeField] public int maxSize;
    [SerializeField] private AnimationCurve sizeDistribution;
    
    
    public static float weightedRandom(float min,float max,AnimationCurve distribution)=>Mathf.Lerp(min,max,distribution.Evaluate(Random.Range(0, 2)));
    public Suspect GenerateSuspect(Scenario _scenario, int _suspectToInnocentThresold)
    {

        HairTestimonial hairTestimonial1 = _scenario.testimonial1 as HairTestimonial;
        HairTestimonial hairTestimonial2 = _scenario.testimonial2 as HairTestimonial;
        
        BeardTestimonial beardTestimonial1 = _scenario.testimonial1 as BeardTestimonial;
        BeardTestimonial beardTestimonial2 = _scenario.testimonial2 as BeardTestimonial;
        
        BodyTestimonial bodyTestimonial1 = _scenario.testimonial1 as BodyTestimonial;
        BodyTestimonial bodyTestimonial2 = _scenario.testimonial2 as BodyTestimonial;
        
        AgeTestimonial ageTestimonial1 = _scenario.testimonial1 as AgeTestimonial;
        AgeTestimonial ageTestimonial2 = _scenario.testimonial2 as AgeTestimonial;
        
        SizeTestimonial sizeTestimonial1 = _scenario.testimonial1 as SizeTestimonial;
        SizeTestimonial sizeTestimonial2 = _scenario.testimonial2 as SizeTestimonial;
        
        
        bool isAMan = _scenario.forceAMan || Random.Range(0,2) == 0;
        
        int generatedAge = GenerateAge(ageTestimonial1,ageTestimonial2,_suspectToInnocentThresold);

        Suspect newSuspect = new Suspect()
        {
            name = isAMan ? NameAndSurnameParser.GetManName() : NameAndSurnameParser.GetWomanName(),
            surname = NameAndSurnameParser.GetSurname(),
            age = generatedAge,
            date = DateTime.Now - TimeSpan.FromDays(Random.Range(0f, 364))- TimeSpan.FromDays(365*generatedAge),
            gender = isAMan ? "Man" : "Woman",
            height = GenerateSize(sizeTestimonial1,sizeTestimonial2,_suspectToInnocentThresold),
            aliveCountWhenEliminated = int.MaxValue,
            bodySize = HaveToGenerateBodySize(bodyTestimonial1,bodyTestimonial2,_suspectToInnocentThresold),
            hairToGenerate = GenerateHair(hairTestimonial1,hairTestimonial2,_suspectToInnocentThresold),
            generateBeard = HaveToGenerateBeard(beardTestimonial1,beardTestimonial2,_suspectToInnocentThresold)
        };
        return newSuspect;
    }

    int GenerateAge(AgeTestimonial ageTestimonial1,AgeTestimonial ageTestimonial2, int _suspectToInnocentThresold)
    {
        if (ageTestimonial1 != null)
        {
            if (_suspectToInnocentThresold <= 2)
            {
                return (int)weightedRandom(ageTestimonial1.isOld ? minAge : minAgeOld, !ageTestimonial1.isOld ? maxAgeYoung : maxAge , ageDistribution);
            }
            return (int)weightedRandom(ageTestimonial1.isOld ? minAgeOld : minAge, !ageTestimonial1.isOld ? maxAge : maxAgeYoung, ageDistribution);
        }
        if (ageTestimonial2 != null)
        {
            if (_suspectToInnocentThresold is <= 4 and >2 )
            {
                return (int)weightedRandom(ageTestimonial2.isOld ? minAge : minAgeOld, !ageTestimonial2.isOld ? maxAgeYoung : maxAge , ageDistribution);
            }
            return (int)weightedRandom(ageTestimonial2.isOld ? minAgeOld : minAge, !ageTestimonial2.isOld ? maxAge : maxAgeYoung, ageDistribution);
        }
        return (int)weightedRandom(minAge, maxAge, ageDistribution);
    }
    int GenerateSize(SizeTestimonial sizeTestimonial1, SizeTestimonial sizeTestimonial2, int _suspectToInnocentThresold)
    {
        if (sizeTestimonial1 != null)
        {
            if (_suspectToInnocentThresold <= 2)
            {
                return (int)weightedRandom(sizeTestimonial1.isTall ? minSize : minSizeTall,
                    sizeTestimonial1.isTall ? maxSizeTiny : maxSize, sizeDistribution);

            }
            return (int)weightedRandom(sizeTestimonial1.isTall?minSizeTall:minSize,sizeTestimonial1.isTall?maxSize:maxSizeTiny, sizeDistribution); 

             }
        if (sizeTestimonial2 != null)
        {
            if (_suspectToInnocentThresold is <= 4  and >2)
            {
                return (int)weightedRandom(sizeTestimonial2.isTall?minSize:minSizeTall,sizeTestimonial2.isTall?maxSizeTiny:maxSize, sizeDistribution);
            }
            return (int)weightedRandom(sizeTestimonial2.isTall?minSizeTall:minSize,sizeTestimonial2.isTall?maxSize:maxSizeTiny, sizeDistribution);
        }
        return (int)weightedRandom(minSize, maxSize, sizeDistribution); 
    }
    bool HaveToGenerateBeard(BeardTestimonial beardTestimonial1, BeardTestimonial beardTestimonial2, int _suspectToInnocentThresold)
    {
        if (beardTestimonial1 != null)
        {
            if (_suspectToInnocentThresold <= 2) return !beardTestimonial1.haveBeard;
            return beardTestimonial1.haveBeard;
            
        }
        if (beardTestimonial2 != null)
        {
            if (_suspectToInnocentThresold is <= 4  and >2) return !beardTestimonial2.haveBeard;
            return beardTestimonial2.haveBeard;
            
        }
        return Random.Range(0, 2) == 0;
    }
    //0:NO impact 1:Skinny 2:Big
    int HaveToGenerateBodySize(BodyTestimonial bodyTestimonial1, BodyTestimonial bodyTestimonial2, int _suspectToInnocentThresold)
    {
        if (bodyTestimonial1 != null)
        {
            if (_suspectToInnocentThresold <= 2) return bodyTestimonial1.isBig?2:1;
            return bodyTestimonial1.isBig?1:2;
        }
        if (bodyTestimonial2 != null)
        {
            if (_suspectToInnocentThresold is <= 4  and >2) return bodyTestimonial1.isBig?1:2;
            return bodyTestimonial2.isBig?1:2;
        }
        return 0;
    }
    (bool,TestimonialHairType) GenerateHair(HairTestimonial hairTestimonial1, HairTestimonial hairTestimonial2, int _suspectToInnocentThresold)
    {
        if (hairTestimonial1 != null)
        {
            if (_suspectToInnocentThresold <= 2) return (false, hairTestimonial1.hairType);
            return (true, hairTestimonial1.hairType);
            
        }
        if (hairTestimonial2 != null)
        {
            if (_suspectToInnocentThresold is <= 4  and >2 ) return (false, hairTestimonial2.hairType);
            return (true, hairTestimonial2.hairType);
            
        }
        return (true,TestimonialHairType.Any);
    }
}

