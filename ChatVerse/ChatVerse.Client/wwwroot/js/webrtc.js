const peers = {};

export async function joinVoice(roomId) {
  const stream = await navigator.mediaDevices.getUserMedia({ audio: true });
  window.localStream = stream;
}

export function handleRemoteOffer(roomId, offerJson) {
}

export function handleRemoteAnswer(roomId, answerJson) {
}

export function handleRemoteCandidate(roomId, candJson) {
}
