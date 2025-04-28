from model import KeyPointClassifier_pb
import json
import csv
import time

keypoint_classifier = KeyPointClassifier_pb()

with open('model/keypoint_classifier/keypoint_classifier_label.csv',
              encoding='utf-8-sig') as f:
        keypoint_classifier_labels = csv.reader(f)
        keypoint_classifier_labels = [row[0] for row in keypoint_classifier_labels]        

def process(data):
    coordinates = [handpoint['coordinate'] for handpoint in data['handpoints']]
    toNN = list()
    for coord in coordinates:
        toNN.append(coord['x'])
        toNN.append(coord['y'])
        toNN.append(coord['z'])
    
    start = time.time()
    handsign_id = keypoint_classifier(toNN)
    end = time.time()
    sec = end-start
    
    print( str(handsign_id) + "   " + str(sec) +" sec")
    
    return keypoint_classifier_labels[handsign_id]

request = '{"handpoints":[{"name":"L_Wrist","coordinate":{"x":0.0,"y":0.0,"z":0.0}},{"name":"L_IndexMetacarpal","coordinate":{"x":-0.01329609751701355,"y":0.035170793533325198,"z":0.02159436047077179}},{"name":"L_IndexProximal","coordinate":{"x":-0.040129393339157107,"y":0.060836851596832278,"z":0.06739071756601334}},{"name":"L_IndexIntermediate","coordinate":{"x":-0.011963456869125367,"y":0.08513742685317993,"z":0.07506822049617768}},{"name":"L_IndexDistal","coordinate":{"x":-0.002597123384475708,"y":0.07720947265625,"z":0.054047927260398868}},{"name":"L_IndexTip","coordinate":{"x":-0.015077173709869385,"y":0.061636924743652347,"z":0.04382753372192383}},{"name":"L_LittleMetacarpal","coordinate":{"x":0.007742255926132202,"y":0.005677700042724609,"z":0.04113207757472992}},{"name":"L_LittleProximal","coordinate":{"x":0.0022933781147003175,"y":0.019312262535095216,"z":0.08442731201648712}},{"name":"L_LittleIntermediate","coordinate":{"x":0.01992693543434143,"y":0.044265568256378177,"z":0.08080540597438812}},{"name":"L_LittleDistal","coordinate":{"x":0.027510136365890504,"y":0.0418543815612793,"z":0.06208561360836029}},{"name":"L_LittleTip","coordinate":{"x":0.015707075595855714,"y":0.02523273229598999,"z":0.053838059306144717}},{"name":"L_MiddleMetacarpal","coordinate":{"x":-0.006292074918746948,"y":0.021826326847076417,"z":0.028705745935440065}},{"name":"L_MiddleProximal","coordinate":{"x":-0.03207749128341675,"y":0.043560028076171878,"z":0.07911279052495957}},{"name":"L_MiddleIntermediate","coordinate":{"x":0.00046584010124206543,"y":0.07160627841949463,"z":0.08073806762695313}},{"name":"L_MiddleDistal","coordinate":{"x":0.009438395500183106,"y":0.06532180309295654,"z":0.05541354417800903}},{"name":"L_MiddleTip","coordinate":{"x":-0.006624937057495117,"y":0.05032473802566528,"z":0.043432727456092837}},{"name":"L_Palm","coordinate":{"x":-0.019184768199920656,"y":0.03269314765930176,"z":0.05390927195549011}},{"name":"L_RingMetacarpal","coordinate":{"x":0.0005990266799926758,"y":0.008984982967376709,"z":0.037331387400627139}},{"name":"L_RingProximal","coordinate":{"x":-0.016360491514205934,"y":0.030730843544006349,"z":0.08382672816514969}},{"name":"L_RingIntermediate","coordinate":{"x":0.011937141418457032,"y":0.05751669406890869,"z":0.08115485310554505}},{"name":"L_RingDistal","coordinate":{"x":0.019755899906158448,"y":0.05257612466812134,"z":0.056200966238975528}},{"name":"L_RingTip","coordinate":{"x":0.004345715045928955,"y":0.03599470853805542,"z":0.04704943299293518}},{"name":"L_ThumbMetacarpal","coordinate":{"x":-0.010540425777435303,"y":0.04600691795349121,"z":0.016232430934906007}},{"name":"L_ThumbProximal","coordinate":{"x":-0.007625013589859009,"y":0.07693004608154297,"z":0.02600419521331787}},{"name":"L_ThumbDistal","coordinate":{"x":0.0058096349239349369,"y":0.08845657110214234,"z":0.05484870821237564}},{"name":"L_ThumbTip","coordinate":{"x":0.017984241247177125,"y":0.08023017644882202,"z":0.07465160638093949}}]}'

data = request.json
print(data)

result = process(data)

print(result)